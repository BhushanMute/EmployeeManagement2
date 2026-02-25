$(document).ready(function () {

    // ========== Toggle Password Visibility ==========
    $('#togglePassword').click(function () {
        const passwordInput = $('#password');
        const icon = $('#togglePasswordIcon');

        if (passwordInput.attr('type') === 'password') {
            passwordInput.attr('type', 'text');
            icon.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            passwordInput.attr('type', 'password');
            icon.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    $('#toggleConfirmPassword').click(function () {
        const confirmPasswordInput = $('#confirmPassword');
        const icon = $('#toggleConfirmPasswordIcon');

        if (confirmPasswordInput.attr('type') === 'password') {
            confirmPasswordInput.attr('type', 'text');
            icon.removeClass('fa-eye').addClass('fa-eye-slash');
        } else {
            confirmPasswordInput.attr('type', 'password');
            icon.removeClass('fa-eye-slash').addClass('fa-eye');
        }
    });

    // ========== Password Strength Checker ==========
    $('#password').on('input', function () {
        const password = $(this).val();
        let strength = 0;
        let strengthText = '';
        let strengthClass = '';

        // Check requirements
        const hasLength = password.length >= 6;
        const hasUpper = /[A-Z]/.test(password);
        const hasLower = /[a-z]/.test(password);
        const hasNumber = /\d/.test(password);
        const hasSpecial = /[@$!%*?&]/.test(password);

        // Update requirement indicators
        updateRequirement('#reqLength', hasLength);
        updateRequirement('#reqUpper', hasUpper);
        updateRequirement('#reqLower', hasLower);
        updateRequirement('#reqNumber', hasNumber);
        updateRequirement('#reqSpecial', hasSpecial);

        // Calculate strength
        if (hasLength) strength += 20;
        if (hasUpper) strength += 20;
        if (hasLower) strength += 20;
        if (hasNumber) strength += 20;
        if (hasSpecial) strength += 20;

        // Set strength indicator
        if (strength <= 20) {
            strengthText = 'Very Weak';
            strengthClass = 'bg-danger';
        } else if (strength <= 40) {
            strengthText = 'Weak';
            strengthClass = 'bg-warning';
        } else if (strength <= 60) {
            strengthText = 'Fair';
            strengthClass = 'bg-info';
        } else if (strength <= 80) {
            strengthText = 'Good';
            strengthClass = 'bg-primary';
        } else {
            strengthText = 'Strong';
            strengthClass = 'bg-success';
        }

        $('#passwordStrength')
            .css('width', strength + '%')
            .removeClass('bg-danger bg-warning bg-info bg-primary bg-success')
            .addClass(strengthClass);
        $('#passwordStrengthText').text(strengthText);

        // Check password match
        checkPasswordMatch();
    });

    // ========== Check Password Match ==========
    $('#confirmPassword').on('input', function () {
        checkPasswordMatch();
    });

    function checkPasswordMatch() {
        const password = $('#password').val();
        const confirmPassword = $('#confirmPassword').val();

        if (confirmPassword.length > 0) {
            if (password === confirmPassword) {
                $('#passwordMatch')
                    .html('<i class="fas fa-check-circle text-success me-1"></i>Passwords match')
                    .removeClass('text-danger')
                    .addClass('text-success');
                $('#confirmPassword').removeClass('is-invalid').addClass('is-valid');
            } else {
                $('#passwordMatch')
                    .html('<i class="fas fa-times-circle text-danger me-1"></i>Passwords do not match')
                    .removeClass('text-success')
                    .addClass('text-danger');
                $('#confirmPassword').removeClass('is-valid').addClass('is-invalid');
            }
        } else {
            $('#passwordMatch').html('');
            $('#confirmPassword').removeClass('is-valid is-invalid');
        }
    }

    function updateRequirement(selector, isMet) {
        const element = $(selector);
        if (isMet) {
            element.addClass('requirement-met text-success');
            element.find('i').removeClass('fa-circle').addClass('fa-check-circle');
        } else {
            element.removeClass('requirement-met text-success');
            element.find('i').removeClass('fa-check-circle').addClass('fa-circle');
        }
    }

    // ========== Form Submission with Loading ==========
    $('#registerForm').on('submit', function () {
        const btn = $('#submitBtn');
        btn.prop('disabled', true);
        btn.html('<span class="spinner-border spinner-border-sm me-2"></span>Creating Account...');
    });

    // ========== Phone Number Formatting ==========
    $('#PhoneNumber').on('input', function () {
        let value = $(this).val().replace(/\D/g, '');
        if (value.length > 10) {
            value = value.substring(0, 10);
        }
        if (value.length >= 6) {
            value = value.substring(0, 3) + '-' + value.substring(3, 6) + '-' + value.substring(6);
        } else if (value.length >= 3) {
            value = value.substring(0, 3) + '-' + value.substring(3);
        }
        $(this).val(value);
    });

    // ========== Real-time validation feedback ==========
    $('input[required]').on('blur', function () {
        if ($(this).val().trim() === '') {
            $(this).addClass('is-invalid');
        } else {
            $(this).removeClass('is-invalid').addClass('is-valid');
        }
    });

    // Email validation
    $('#Email').on('blur', function () {
        const email = $(this).val();
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (email && !emailRegex.test(email)) {
            $(this).addClass('is-invalid').removeClass('is-valid');
        } else if (email) {
            $(this).addClass('is-valid').removeClass('is-invalid');
        }
    });

    // Username validation (min 3 chars)
    $('#Username').on('blur', function () {
        const username = $(this).val();

        if (username && username.length < 3) {
            $(this).addClass('is-invalid').removeClass('is-valid');
        } else if (username) {
            $(this).addClass('is-valid').removeClass('is-invalid');
        }
    });
});