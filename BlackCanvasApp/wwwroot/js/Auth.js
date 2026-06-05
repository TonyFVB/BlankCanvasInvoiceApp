"use strict";

// ── Utilidades DOM ────────────────────────────────────────────
function qs(selector, root = document) {
    const el = root.querySelector(selector);
    if (!el) throw new Error(`Elemento no encontrado: ${selector}`);
    return el;
}

function qsAll(selector, root = document) {
    return Array.from(root.querySelectorAll(selector));
}

function show(el) { el?.classList.remove('hidden'); }
function hide(el) { el?.classList.add('hidden'); }

function setLoading(btn, loading) {
    if (!btn) return;
    btn.classList.toggle('loading', loading);
    btn.disabled = loading;
}

// ── Validación ────────────────────────────────────────────────
function clearError(groupId) {
    const group = document.getElementById(groupId);
    group?.classList.remove('has-error');
    const errorEl = group?.querySelector('.input-error');
    if (errorEl) errorEl.textContent = '';
}

function setError(groupId, message) {
    const group = document.getElementById(groupId);
    group?.classList.add('has-error');
    const errorEl = group?.querySelector('.input-error');
    if (errorEl) errorEl.textContent = message;
}

function isValidEmail(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim());
}

function isValidPassword(password) {
    return /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/.test(password);
}

// ── Antiforgery ───────────────────────────────────────────────
function getAntiForgeryToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '';
}

// ── Toggle visibilidad contraseña ─────────────────────────────
function initPasswordToggles() {
    const toggles = qsAll('.toggle-password');
    toggles.forEach(btn => {
        btn.addEventListener('click', () => {
            const targetId = btn.dataset['target'] ?? '';
            const input = document.getElementById(targetId);
            if (!input) return;
            const isPassword = input.type === 'password';
            input.type = isPassword ? 'text' : 'password';
            btn.classList.toggle('visible', isPassword);
            btn.setAttribute('aria-label', isPassword ? 'Ocultar contraseña' : 'Mostrar contraseña');
        });
    });
}

// ── Indicador de fortaleza de contraseña ──────────────────────
function getStrengthLevel(password) {
    let score = 0;
    if (password.length >= 8) score++;
    if (password.length >= 12) score++;
    if (/[A-Z]/.test(password)) score++;
    if (/[a-z]/.test(password)) score++;
    if (/\d/.test(password)) score++;
    if (/[^A-Za-z0-9]/.test(password)) score++;
    if (score <= 2) return { level: 'weak', score: 1, label: 'Débil' };
    if (score <= 3) return { level: 'fair', score: 2, label: 'Regular' };
    if (score <= 4) return { level: 'good', score: 3, label: 'Buena' };
    return { level: 'strong', score: 4, label: 'Fuerte' };
}

function updateStrengthBars(password) {
    const container = document.getElementById('password-strength');
    const labelEl = document.getElementById('strength-label');
    const bars = [1, 2, 3, 4].map(i => document.getElementById(`bar-${i}`));
    if (!container || !labelEl) return;

    if (!password) {
        container.classList.remove('visible');
        bars.forEach(b => b?.classList.remove('active-weak', 'active-fair', 'active-good', 'active-strong'));
        return;
    }

    container.classList.add('visible');
    const { score, label, level } = getStrengthLevel(password);
    bars.forEach((bar, idx) => {
        if (!bar) return;
        bar.classList.remove('active-weak', 'active-fair', 'active-good', 'active-strong');
        if (idx < score) bar.classList.add(`active-${level}`);
    });
    labelEl.textContent = label;
}

function initStrengthIndicator() {
    // El input puede tener id="reg-password" (asignado manualmente en Register.cshtml)
    const passwordInput = document.getElementById('reg-password');
    passwordInput?.addEventListener('input', () => {
        updateStrengthBars(passwordInput.value);
    });
}

// ── Formulario: LOGIN ─────────────────────────────────────────
// Solo existe en Login.cshtml
function initLoginForm() {
    const form = document.getElementById('form-login');
    const btn = document.getElementById('btn-login');
    if (!form) return; // no estamos en Login.cshtml

    form.addEventListener('submit', () => {
        if (btn) btn.classList.add('loading');
    });
}

// ── Formulario: REGISTER ──────────────────────────────────────
// Solo existe en Register.cshtml
function initRegisterForm() {
    const form = document.getElementById('form-register');
    const btn = document.getElementById('btn-register');
    if (!form) return; // no estamos en Register.cshtml

    form.addEventListener('submit', () => {
        if (btn) btn.classList.add('loading');
    });
}

// ── Formulario: FORGOT PASSWORD ───────────────────────────────
// Solo existe en Login.cshtml (modal)
function initForgotPasswordForm() {
    const form = document.getElementById('form-forgot');
    const btnForgot = document.getElementById('btn-forgot');
    const alertSuccess = document.getElementById('alert-forgot-success');
    const modalOverlay = document.getElementById('modal-forgot');
    if (!form) return; // no estamos en Login.cshtml

    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        hide(alertSuccess);
        clearError('group-forgot-email');

        const email = document.getElementById('forgot-email')?.value.trim() ?? '';

        if (!email) {
            setError('group-forgot-email', 'El correo es obligatorio.');
            return;
        }
        if (!isValidEmail(email)) {
            setError('group-forgot-email', 'Ingresa un correo válido.');
            return;
        }

        setLoading(btnForgot, true);

        try {
            const response = await fetch('/Auth/ForgotPassword', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                    'RequestVerificationToken': getAntiForgeryToken(),
                },
                body: `email=${encodeURIComponent(email)}`,
            });
            await response.json();
        } catch (_) { /* siempre mostramos éxito */ }

        setLoading(btnForgot, false);

        if (alertSuccess) {
            alertSuccess.textContent = 'Si el correo existe, recibirás un enlace en breve.';
            show(alertSuccess);
        }

        setTimeout(() => closeForgotModal(modalOverlay, form), 3000);
    });
}

// ── Modal: FORGOT PASSWORD ────────────────────────────────────
// Solo existe en Login.cshtml
function initModal() {
    const modal = document.getElementById('modal-forgot');
    const btnClose = document.getElementById('btn-close-modal');
    const linkForgot = document.getElementById('link-forgot');
    if (!modal) return; // no estamos en Login.cshtml

    linkForgot?.addEventListener('click', (e) => {
        e.preventDefault();
        show(modal);
        document.getElementById('forgot-email')?.focus();
    });

    btnClose?.addEventListener('click', () => closeForgotModal(modal));

    modal.addEventListener('click', (e) => {
        if (e.target === modal) closeForgotModal(modal);
    });

    document.addEventListener('keydown', (e) => {
        if (e.key === 'Escape' && !modal.classList.contains('hidden'))
            closeForgotModal(modal);
    });
}

function closeForgotModal(modal, form) {
    hide(modal);
    form?.reset();
    clearError('group-forgot-email');
    const alertSuccess = document.getElementById('alert-forgot-success');
    if (alertSuccess) hide(alertSuccess);
}

// ── Navegación entre vistas ───────────────────────────────────
// Los botones existen en vistas distintas — cada uno se verifica
function initNavigation() {
    // En Login.cshtml — botón "Create Account" → navega a Register
    const btnGoRegister = document.getElementById('btn-go-register');
    if (btnGoRegister) {
        btnGoRegister.addEventListener('click', () => {
            window.location.href = '/Auth/Register';
        });
    }

    // En Register.cshtml — botón "Back to Login" → navega a Login
    const btnGoLogin = document.getElementById('btn-go-login');
    if (btnGoLogin) {
        btnGoLogin.addEventListener('click', () => {
            window.location.href = '/Auth/Login';
        });
    }
}

// ── INIT ──────────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
    initPasswordToggles();
    initStrengthIndicator();
    initLoginForm();
    initRegisterForm();
    initForgotPasswordForm();
    initModal();
    initNavigation();
});