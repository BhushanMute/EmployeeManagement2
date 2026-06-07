from __future__ import annotations

import html
import re
import sys
import xml.etree.ElementTree as ET
from collections import defaultdict
from dataclasses import dataclass, field
from datetime import datetime
from pathlib import Path

from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_LEFT
from reportlab.lib.pagesizes import letter
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import inch
from reportlab.platypus import (
    KeepTogether,
    ListFlowable,
    ListItem,
    PageBreak,
    Paragraph,
    SimpleDocTemplate,
    Spacer,
    Table,
    TableStyle,
)


ROOT = Path(__file__).resolve().parents[1]
OUT_DIR = ROOT / "docs"
OUT_FILE = OUT_DIR / "EmployeeManagement2_Full_Project_Structure.pdf"

EXCLUDED_DIRS = {
    ".git",
    ".vs",
    "bin",
    "obj",
    "run-logs",
    "Logs",
    "log",
    "logs",
    "node_modules",
    "packages",
    "TestResults",
    "__pycache__",
}

EXCLUDED_PARTS = {
    ("wwwroot", "uploads"),
}

TYPE_RE = re.compile(
    r"(?m)^\s*"
    r"(?P<attrs>(?:\[[^\]]+\]\s*)*)"
    r"(?P<mods>(?:(?:public|private|protected|internal|static|abstract|sealed|partial|readonly|unsafe|new)\s+)*)"
    r"(?P<kind>class|interface|struct|enum|record(?:\s+(?:class|struct))?)\s+"
    r"(?P<name>[A-Za-z_][A-Za-z0-9_`]*(?:\s*<[^>{};]+>)?)"
    r"(?P<tail>[^{};]*)"
)

NAMESPACE_RE = re.compile(r"(?m)^\s*namespace\s+([A-Za-z_][A-Za-z0-9_.]*)\s*[;{]")

METHOD_RE = re.compile(
    r"(?m)^\s*(?:\[[^\]]+\]\s*)*"
    r"(?P<mods>(?:(?:public|private|protected|internal|static|virtual|override|abstract|async|sealed|new|extern|partial)\s+)*)"
    r"(?P<ret>[A-Za-z_][A-Za-z0-9_<>,\.\?\[\]\s]*(?:\s*\?)?)\s+"
    r"(?P<name>[A-Za-z_][A-Za-z0-9_]*)\s*"
    r"\((?P<params>[^;{}()]*)\)\s*(?:where\s+[^{]+)?\s*(?:=>|{|;)"
)

PROPERTY_RE = re.compile(
    r"(?m)^\s*(?:\[[^\]]+\]\s*)*"
    r"(?P<mods>(?:(?:public|private|protected|internal|static|virtual|override|abstract|required|new|init|readonly)\s+)*)"
    r"(?P<type>[A-Za-z_][A-Za-z0-9_<>,\.\?\[\]\s]*(?:\s*\?)?)\s+"
    r"(?P<name>[A-Za-z_][A-Za-z0-9_]*)\s*"
    r"\{\s*(?:get|set|init)\b"
)

FIELD_RE = re.compile(
    r"(?m)^\s*(?:\[[^\]]+\]\s*)*"
    r"(?P<mods>(?:(?:public|private|protected|internal|static|readonly|const|required|new)\s+)*)"
    r"(?P<type>[A-Za-z_][A-Za-z0-9_<>,\.\?\[\]\s]*(?:\s*\?)?)\s+"
    r"(?P<name>[A-Za-z_][A-Za-z0-9_]*)\s*(?:=|;)"
)


@dataclass
class TypeInfo:
    file: str
    project: str
    namespace: str
    kind: str
    name: str
    modifiers: str
    inherits: str = ""
    attributes: list[str] = field(default_factory=list)
    constructors: list[str] = field(default_factory=list)
    methods: list[str] = field(default_factory=list)
    properties: list[str] = field(default_factory=list)
    fields: list[str] = field(default_factory=list)


def rel(path: Path) -> str:
    return str(path.relative_to(ROOT)).replace("\\", "/")


def is_excluded(path: Path) -> bool:
    rel_parts = path.relative_to(ROOT).parts if path != ROOT else ()
    if any(part in EXCLUDED_DIRS for part in rel_parts):
        return True
    lowered = tuple(part.lower() for part in rel_parts)
    for excluded in EXCLUDED_PARTS:
        if all(part.lower() in lowered for part in excluded):
            if lowered.index(excluded[0]) < len(lowered) - 1:
                return True
    return False


def iter_files() -> list[Path]:
    files: list[Path] = []
    for path in ROOT.rglob("*"):
        if path.is_dir() or is_excluded(path):
            continue
        files.append(path)
    return sorted(files, key=lambda p: rel(p).lower())


def strip_comments(source: str) -> str:
    source = re.sub(r"/\*.*?\*/", "", source, flags=re.S)
    source = re.sub(r"(?m)//.*$", "", source)
    return source


def find_matching_brace(source: str, open_index: int) -> int:
    depth = 0
    for index in range(open_index, len(source)):
        char = source[index]
        if char == "{":
            depth += 1
        elif char == "}":
            depth -= 1
            if depth == 0:
                return index
    return len(source)


def namespace_at(source: str, index: int) -> str:
    namespace = ""
    for match in NAMESPACE_RE.finditer(source):
        if match.start() <= index:
            namespace = match.group(1)
        else:
            break
    return namespace


def project_for(path: Path) -> str:
    for parent in [path.parent, *path.parents]:
        if parent == ROOT.parent:
            break
        projects = list(parent.glob("*.csproj"))
        if projects:
            return projects[0].stem
    return ROOT.name


def compact(text: str, limit: int = 150) -> str:
    text = re.sub(r"\s+", " ", text.strip())
    if len(text) > limit:
        return text[: limit - 3].rstrip() + "..."
    return text


def parse_attributes(attr_text: str) -> list[str]:
    attrs = []
    for item in re.findall(r"\[([^\]]+)\]", attr_text):
        attrs.append(compact(item, 90))
    return attrs


def parse_type_members(body: str, type_name: str) -> tuple[list[str], list[str], list[str], list[str]]:
    constructors: list[str] = []
    methods: list[str] = []
    properties: list[str] = []
    fields: list[str] = []

    constructor_re = re.compile(
        rf"(?m)^\s*(?:\[[^\]]+\]\s*)*(?P<mods>(?:(?:public|private|protected|internal)\s+)*)"
        rf"{re.escape(type_name.split('<')[0].strip())}\s*\((?P<params>[^;{{}}()]*)\)\s*(?:=>|{{|;)"
    )

    for match in constructor_re.finditer(body):
        mods = compact(match.group("mods"))
        params = compact(match.group("params"), 120)
        constructors.append(compact(f"{mods} {type_name}({params})".strip()))

    for match in PROPERTY_RE.finditer(body):
        mods = compact(match.group("mods"))
        if "public" not in mods and "protected" not in mods and "internal" not in mods:
            continue
        properties.append(compact(f"{mods} {match.group('type').strip()} {match.group('name')}".strip()))

    for match in FIELD_RE.finditer(body):
        mods = compact(match.group("mods"))
        if "public" not in mods and "protected" not in mods and "internal" not in mods:
            continue
        signature = compact(f"{mods} {match.group('type').strip()} {match.group('name')}".strip())
        if signature not in properties:
            fields.append(signature)

    control_names = {
        "if",
        "for",
        "foreach",
        "while",
        "switch",
        "catch",
        "using",
        "lock",
        "return",
        "new",
    }
    for match in METHOD_RE.finditer(body):
        name = match.group("name")
        if name in control_names or name == type_name:
            continue
        mods = compact(match.group("mods"))
        if "public" not in mods and "protected" not in mods and "internal" not in mods:
            continue
        ret = compact(match.group("ret"), 80)
        params = compact(match.group("params"), 130)
        methods.append(compact(f"{mods} {ret} {name}({params})".strip(), 180))

    return (
        sorted(dict.fromkeys(constructors)),
        sorted(dict.fromkeys(methods)),
        sorted(dict.fromkeys(properties)),
        sorted(dict.fromkeys(fields)),
    )


def parse_csharp_file(path: Path) -> list[TypeInfo]:
    try:
        raw = path.read_text(encoding="utf-8-sig")
    except UnicodeDecodeError:
        raw = path.read_text(encoding="latin-1", errors="ignore")
    source = strip_comments(raw)
    type_infos: list[TypeInfo] = []

    for match in TYPE_RE.finditer(source):
        kind = compact(match.group("kind"))
        name = compact(match.group("name"))
        tail = match.group("tail") or ""
        inherits = ""
        if ":" in tail:
            inherits = compact(tail.split(":", 1)[1].strip(" \r\n\t("), 180)

        body = ""
        brace_index = source.find("{", match.end())
        if brace_index != -1 and brace_index < match.end() + 300:
            end_index = find_matching_brace(source, brace_index)
            body = source[brace_index + 1 : end_index]

        constructors, methods, properties, fields = parse_type_members(body, name)
        type_infos.append(
            TypeInfo(
                file=rel(path),
                project=project_for(path),
                namespace=namespace_at(source, match.start()),
                kind=kind,
                name=name,
                modifiers=compact(match.group("mods")),
                inherits=inherits,
                attributes=parse_attributes(match.group("attrs")),
                constructors=constructors,
                methods=methods,
                properties=properties,
                fields=fields,
            )
        )

    return type_infos


def read_csproj(path: Path) -> dict[str, object]:
    data: dict[str, object] = {
        "path": rel(path),
        "name": path.stem,
        "target_frameworks": [],
        "packages": [],
        "project_refs": [],
    }
    try:
        root = ET.parse(path).getroot()
    except ET.ParseError:
        return data

    frameworks: set[str] = set()
    packages: list[str] = []
    refs: list[str] = []
    for elem in root.iter():
        tag = elem.tag.split("}")[-1]
        if tag in {"TargetFramework", "TargetFrameworks"} and elem.text:
            frameworks.update(part.strip() for part in elem.text.split(";") if part.strip())
        if tag == "PackageReference":
            include = elem.attrib.get("Include", "")
            version = elem.attrib.get("Version", "")
            if include:
                packages.append(f"{include} {version}".strip())
        if tag == "ProjectReference":
            include = elem.attrib.get("Include", "")
            if include:
                refs.append(include.replace("\\", "/"))

    data["target_frameworks"] = sorted(frameworks)
    data["packages"] = sorted(packages, key=str.lower)
    data["project_refs"] = sorted(refs, key=str.lower)
    return data


def make_tree(files: list[Path]) -> list[str]:
    tree: dict[str, dict] = {}
    for path in files:
        parts = rel(path).split("/")
        node = tree
        for part in parts:
            node = node.setdefault(part, {})

    lines: list[str] = [ROOT.name]

    def walk(node: dict, prefix: str = "") -> None:
        items = sorted(node.items(), key=lambda item: (bool(item[1]), item[0].lower()))
        for index, (name, children) in enumerate(items):
            last = index == len(items) - 1
            branch = "`-- " if last else "|-- "
            lines.append(prefix + branch + name)
            if children:
                walk(children, prefix + ("    " if last else "|   "))

    walk(tree)
    return lines


def classify_file(path: Path) -> str:
    parts = path.parts
    suffix = path.suffix.lower()
    text = rel(path).lower()
    if suffix == ".cs":
        if "controllers" in text:
            return "Controllers"
        if "models" in text or "viewmodels" in text:
            return "Models and ViewModels"
        if "repositories" in text:
            return "Repositories"
        if "services" in text:
            return "Services"
        if "middleware" in text:
            return "Middleware"
        return "C# Source"
    if suffix == ".cshtml":
        return "MVC Views"
    if suffix == ".sql":
        return "SQL Scripts"
    if suffix in {".json", ".config", ".xml", ".csproj", ".sln"}:
        return "Configuration and Project Files"
    if suffix in {".css", ".js", ".html", ".scss"}:
        return "Client Assets"
    if suffix in {".md", ".txt"}:
        return "Documentation"
    return "Other"


def sql_procedure_names(path: Path) -> list[str]:
    try:
        text = path.read_text(encoding="utf-8-sig", errors="ignore")
    except Exception:
        return []
    names = re.findall(
        r"(?i)\b(?:CREATE|ALTER|CREATE\s+OR\s+ALTER)\s+(?:PROCEDURE|PROC)\s+(?:\[dbo\]\.)?\[?([A-Za-z0-9_]+)\]?",
        text,
    )
    return sorted(dict.fromkeys(names), key=str.lower)


def view_model(path: Path) -> str:
    try:
        text = path.read_text(encoding="utf-8-sig", errors="ignore")
    except Exception:
        return ""
    match = re.search(r"(?m)^\s*@model\s+(.+)$", text)
    return compact(match.group(1), 140) if match else ""


def p(text: str, style: ParagraphStyle) -> Paragraph:
    return Paragraph(html.escape(text), style)


def bullet_list(items: list[str], style: ParagraphStyle, max_items: int | None = None) -> ListFlowable:
    shown = items if max_items is None else items[:max_items]
    list_items = [ListItem(p(item, style), leftIndent=10) for item in shown]
    if max_items is not None and len(items) > max_items:
        list_items.append(ListItem(p(f"... {len(items) - max_items} more", style), leftIndent=10))
    return ListFlowable(list_items, bulletType="bullet", leftIndent=14, bulletFontSize=6)


def header_footer(canvas, doc):
    canvas.saveState()
    width, height = letter
    canvas.setFont("Helvetica", 7)
    canvas.setFillColor(colors.HexColor("#64748B"))
    canvas.drawString(doc.leftMargin, 0.45 * inch, "EmployeeManagement2 HRMS - Project Structure Inventory")
    canvas.drawRightString(width - doc.rightMargin, 0.45 * inch, f"Page {doc.page}")
    canvas.restoreState()


def build_pdf(files: list[Path], types: list[TypeInfo]) -> None:
    OUT_DIR.mkdir(parents=True, exist_ok=True)

    doc = SimpleDocTemplate(
        str(OUT_FILE),
        pagesize=letter,
        rightMargin=0.55 * inch,
        leftMargin=0.55 * inch,
        topMargin=0.65 * inch,
        bottomMargin=0.65 * inch,
        title="EmployeeManagement2 Full Project Structure",
        author="Codex",
    )

    base = getSampleStyleSheet()
    styles = {
        "title": ParagraphStyle(
            "ReportTitle",
            parent=base["Title"],
            fontName="Helvetica-Bold",
            fontSize=22,
            leading=26,
            alignment=TA_CENTER,
            textColor=colors.HexColor("#0F172A"),
            spaceAfter=10,
        ),
        "subtitle": ParagraphStyle(
            "Subtitle",
            parent=base["Normal"],
            fontName="Helvetica",
            fontSize=9,
            leading=13,
            alignment=TA_CENTER,
            textColor=colors.HexColor("#475569"),
            spaceAfter=18,
        ),
        "h1": ParagraphStyle(
            "Heading1Custom",
            parent=base["Heading1"],
            fontName="Helvetica-Bold",
            fontSize=15,
            leading=18,
            textColor=colors.HexColor("#1E3A8A"),
            spaceBefore=12,
            spaceAfter=7,
        ),
        "h2": ParagraphStyle(
            "Heading2Custom",
            parent=base["Heading2"],
            fontName="Helvetica-Bold",
            fontSize=11,
            leading=14,
            textColor=colors.HexColor("#0F172A"),
            spaceBefore=8,
            spaceAfter=4,
        ),
        "body": ParagraphStyle(
            "BodyCustom",
            parent=base["BodyText"],
            fontName="Helvetica",
            fontSize=8,
            leading=10.5,
            textColor=colors.HexColor("#111827"),
            spaceAfter=4,
        ),
        "small": ParagraphStyle(
            "Small",
            parent=base["BodyText"],
            fontName="Helvetica",
            fontSize=6.5,
            leading=8.2,
            textColor=colors.HexColor("#111827"),
            spaceAfter=2,
        ),
        "mono": ParagraphStyle(
            "Mono",
            parent=base["BodyText"],
            fontName="Courier",
            fontSize=5.7,
            leading=6.8,
            textColor=colors.HexColor("#111827"),
            spaceAfter=0.8,
        ),
        "muted": ParagraphStyle(
            "Muted",
            parent=base["BodyText"],
            fontName="Helvetica",
            fontSize=7,
            leading=9,
            textColor=colors.HexColor("#64748B"),
            spaceAfter=3,
        ),
    }

    story: list = []
    generated_at = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    project_files = sorted(ROOT.glob("*.sln")) + sorted(ROOT.rglob("*.csproj"))
    project_infos = [read_csproj(path) for path in project_files if not is_excluded(path)]
    file_categories: dict[str, int] = defaultdict(int)
    for file in files:
        file_categories[classify_file(file)] += 1

    story.append(p("EmployeeManagement2 HRMS", styles["title"]))
    story.append(
        p(
            "Full solution explorer structure and C# class inventory for sharing with another AI module.",
            styles["subtitle"],
        )
    )

    summary_rows = [
        ["Generated", generated_at],
        ["Workspace", str(ROOT)],
        ["Included files", str(len(files))],
        ["C# files scanned", str(sum(1 for f in files if f.suffix.lower() == ".cs"))],
        ["Discovered C# types", str(len(types))],
        ["Projects", ", ".join(info["name"] for info in project_infos if info.get("name")) or "None found"],
    ]
    summary_table = Table(
        [[p(str(a), styles["small"]), p(str(b), styles["small"])] for a, b in summary_rows],
        colWidths=[1.55 * inch, 5.55 * inch],
        hAlign="LEFT",
    )
    summary_table.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (0, -1), colors.HexColor("#E2E8F0")),
                ("BOX", (0, 0), (-1, -1), 0.4, colors.HexColor("#CBD5E1")),
                ("INNERGRID", (0, 0), (-1, -1), 0.25, colors.HexColor("#E2E8F0")),
                ("VALIGN", (0, 0), (-1, -1), "TOP"),
                ("LEFTPADDING", (0, 0), (-1, -1), 5),
                ("RIGHTPADDING", (0, 0), (-1, -1), 5),
                ("TOPPADDING", (0, 0), (-1, -1), 4),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 4),
            ]
        )
    )
    story.append(summary_table)
    story.append(Spacer(1, 8))
    story.append(
        p(
            "Excluded folders: .git, .vs, bin, obj, run-logs, Logs, node_modules, packages, TestResults, and wwwroot/uploads.",
            styles["muted"],
        )
    )
    story.append(PageBreak())

    story.append(p("1. Solution Summary", styles["h1"]))
    category_rows = [["Category", "File Count"]]
    for category, count in sorted(file_categories.items(), key=lambda item: item[0]):
        category_rows.append([category, str(count)])
    category_table = Table(
        [[p(a, styles["small"]), p(b, styles["small"])] for a, b in category_rows],
        colWidths=[4.8 * inch, 1.2 * inch],
        repeatRows=1,
        hAlign="LEFT",
    )
    category_table.setStyle(
        TableStyle(
            [
                ("BACKGROUND", (0, 0), (-1, 0), colors.HexColor("#1E3A8A")),
                ("TEXTCOLOR", (0, 0), (-1, 0), colors.white),
                ("BOX", (0, 0), (-1, -1), 0.4, colors.HexColor("#CBD5E1")),
                ("INNERGRID", (0, 0), (-1, -1), 0.25, colors.HexColor("#E2E8F0")),
                ("VALIGN", (0, 0), (-1, -1), "TOP"),
                ("LEFTPADDING", (0, 0), (-1, -1), 5),
                ("RIGHTPADDING", (0, 0), (-1, -1), 5),
                ("TOPPADDING", (0, 0), (-1, -1), 4),
                ("BOTTOMPADDING", (0, 0), (-1, -1), 4),
            ]
        )
    )
    story.append(category_table)

    story.append(p("2. Project Files and Dependencies", styles["h1"]))
    for info in project_infos:
        story.append(p(str(info["name"]), styles["h2"]))
        story.append(p(f"Path: {info['path']}", styles["small"]))
        story.append(p("Target frameworks: " + (", ".join(info["target_frameworks"]) or "Not specified"), styles["small"]))
        packages = info["packages"]
        refs = info["project_refs"]
        if packages:
            story.append(p("Package references:", styles["small"]))
            story.append(bullet_list(packages, styles["small"], max_items=None))
        if refs:
            story.append(p("Project references:", styles["small"]))
            story.append(bullet_list(refs, styles["small"], max_items=None))

    story.append(PageBreak())
    story.append(p("3. Solution Explorer Tree", styles["h1"]))
    story.append(p("Full included file tree, generated after excluding build/runtime noise.", styles["muted"]))
    for line in make_tree(files):
        story.append(p(line, styles["mono"]))

    story.append(PageBreak())
    story.append(p("4. C# Type Inventory", styles["h1"]))
    story.append(
        p(
            "Each discovered C# class, interface, record, struct, or enum is listed with namespace, file, inheritance, and public/internal/protected members found by static parsing.",
            styles["muted"],
        )
    )

    by_project: dict[str, list[TypeInfo]] = defaultdict(list)
    for type_info in types:
        by_project[type_info.project].append(type_info)

    for project in sorted(by_project):
        story.append(p(project, styles["h2"]))
        for type_info in sorted(by_project[project], key=lambda item: (item.file.lower(), item.name.lower())):
            title = f"{type_info.kind} {type_info.name}"
            if type_info.namespace:
                title += f" ({type_info.namespace})"
            block: list = [p(title, styles["small"])]
            block.append(p(f"File: {type_info.file}", styles["muted"]))
            if type_info.modifiers:
                block.append(p(f"Modifiers: {type_info.modifiers}", styles["small"]))
            if type_info.inherits:
                block.append(p(f"Inherits / implements: {type_info.inherits}", styles["small"]))
            if type_info.attributes:
                block.append(p("Attributes: " + "; ".join(type_info.attributes), styles["small"]))
            if type_info.properties:
                block.append(p(f"Properties ({len(type_info.properties)}):", styles["small"]))
                block.append(bullet_list(type_info.properties, styles["small"], max_items=None))
            if type_info.fields:
                block.append(p(f"Fields ({len(type_info.fields)}):", styles["small"]))
                block.append(bullet_list(type_info.fields, styles["small"], max_items=None))
            if type_info.constructors:
                block.append(p(f"Constructors ({len(type_info.constructors)}):", styles["small"]))
                block.append(bullet_list(type_info.constructors, styles["small"], max_items=None))
            if type_info.methods:
                block.append(p(f"Methods ({len(type_info.methods)}):", styles["small"]))
                block.append(bullet_list(type_info.methods, styles["small"], max_items=None))
            if not (type_info.properties or type_info.fields or type_info.constructors or type_info.methods):
                block.append(p("No public/internal/protected members discovered by parser.", styles["muted"]))
            story.append(KeepTogether(block[:4]))
            story.extend(block[4:])
            story.append(Spacer(1, 4))

    story.append(PageBreak())
    story.append(p("5. MVC Views", styles["h1"]))
    views = [file for file in files if file.suffix.lower() == ".cshtml"]
    if views:
        rows = [["View Path", "Model"]]
        for view in views:
            rows.append([rel(view), view_model(view) or "None declared"])
        table = Table(
            [[p(a, styles["small"]), p(b, styles["small"])] for a, b in rows],
            colWidths=[4.4 * inch, 2.6 * inch],
            repeatRows=1,
            hAlign="LEFT",
        )
        table.setStyle(
            TableStyle(
                [
                    ("BACKGROUND", (0, 0), (-1, 0), colors.HexColor("#334155")),
                    ("TEXTCOLOR", (0, 0), (-1, 0), colors.white),
                    ("BOX", (0, 0), (-1, -1), 0.4, colors.HexColor("#CBD5E1")),
                    ("INNERGRID", (0, 0), (-1, -1), 0.25, colors.HexColor("#E2E8F0")),
                    ("VALIGN", (0, 0), (-1, -1), "TOP"),
                    ("LEFTPADDING", (0, 0), (-1, -1), 4),
                    ("RIGHTPADDING", (0, 0), (-1, -1), 4),
                    ("TOPPADDING", (0, 0), (-1, -1), 3),
                    ("BOTTOMPADDING", (0, 0), (-1, -1), 3),
                ]
            )
        )
        story.append(table)
    else:
        story.append(p("No Razor views found.", styles["body"]))

    story.append(PageBreak())
    story.append(p("6. SQL Scripts and Stored Procedures", styles["h1"]))
    sql_files = [file for file in files if file.suffix.lower() == ".sql"]
    if sql_files:
        for sql_file in sql_files:
            procedures = sql_procedure_names(sql_file)
            story.append(p(rel(sql_file), styles["h2"]))
            if procedures:
                story.append(p("Procedures discovered:", styles["small"]))
                story.append(bullet_list(procedures, styles["small"], max_items=None))
            else:
                story.append(p("No procedure declarations detected in this script.", styles["muted"]))
    else:
        story.append(p("No SQL scripts found.", styles["body"]))

    story.append(PageBreak())
    story.append(p("7. File Index", styles["h1"]))
    for category in sorted(file_categories):
        story.append(p(category, styles["h2"]))
        for file in [item for item in files if classify_file(item) == category]:
            story.append(p(rel(file), styles["mono"]))

    doc.build(story, onFirstPage=header_footer, onLaterPages=header_footer)


def main() -> int:
    if not ROOT.exists():
        print(f"Project root not found: {ROOT}", file=sys.stderr)
        return 1

    files = iter_files()
    types: list[TypeInfo] = []
    for path in files:
        if path.suffix.lower() == ".cs":
            types.extend(parse_csharp_file(path))

    build_pdf(files, types)
    print(OUT_FILE)
    print(f"Included files: {len(files)}")
    print(f"C# types: {len(types)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
