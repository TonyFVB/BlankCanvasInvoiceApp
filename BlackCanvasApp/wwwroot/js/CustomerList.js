let currentMode = 'create';   // 'create' | 'edit'
let deleteTargetId = null;
let currentStatusTab = '';

// ── Antiforgery ───────────────────────────────────────────────
function getToken() {
    return document.querySelector('input[name="__RequestVerificationToken"]')?.value ?? '';
}

// ── Toast ─────────────────────────────────────────────────────
function showToast(message, type = 'success') {
    const container = document.getElementById('toastContainer');
    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    toast.innerHTML = `<span>${type === 'success' ? '✓' : '✕'}</span> ${message}`;
    container.appendChild(toast);
    setTimeout(() => toast.remove(), 3500);
}

// ── Filtro por búsqueda de texto ──────────────────────────────
function filterTable() {
    const search = document.getElementById('searchInput').value.toLowerCase();
    const rows = document.querySelectorAll('#customerTableBody tr[data-id]');

    rows.forEach(row => {
        const name = row.dataset.name ?? '';
        const status = row.dataset.status ?? '';
        const matchSearch = name.includes(search);
        const matchStatus = currentStatusTab === '' || status === currentStatusTab;
        row.style.display = matchSearch && matchStatus ? '' : 'none';
    });
}

// ── Filtro por tab de status ──────────────────────────────────
function setStatusTab(btn) {
    document.querySelectorAll('.status-tab').forEach(t => t.classList.remove('active'));
    btn.classList.add('active');
    currentStatusTab = btn.dataset.status ?? '';
    filterTable();
}

// ── Dropdowns ─────────────────────────────────────────────────
function closeAllDropdowns() {
    document.querySelectorAll('.actions-dropdown.show, .status-dropdown.show, .services-popover.show')
        .forEach(d => d.classList.remove('show'));
}

document.addEventListener('click', () => closeAllDropdowns());

function toggleActionsMenu(e, id) {
    //e.stopPropagation();
    //console.log('click', id);
    //closeAllDropdowns();
    //document.getElementById(`actions-dd-${id}`)?.classList.toggle('show');
    e.stopPropagation();
    const dropdown = document.getElementById(`actions-dd-${id}`);
    const isOpen = dropdown?.classList.contains('show');
    closeAllDropdowns();
    if (!isOpen) dropdown?.classList.add('show');
}

function toggleStatusDropdown(e, id, currentStatus) {
    //e.stopPropagation();
    //closeAllDropdowns();
    //document.getElementById(`status-dd-${id}`)?.classList.toggle('show');
    e.stopPropagation();
    const dropdown = document.getElementById(`status-dd-${id}`);
    const isOpen = dropdown?.classList.contains('show');
    closeAllDropdowns();
    if (!isOpen) dropdown?.classList.add('show');
}

// ── Popup de servicios en listado ─────────────────────────────
function toggleServicesPopover(e, customerId) {
    e.stopPropagation();
    const popover = document.getElementById(`services-popover-${customerId}`);
    const isOpen = popover?.classList.contains('show');
    closeAllDropdowns();
    if (!isOpen) popover?.classList.add('show');
}

function closeServicesPopover(customerId) {
    document.getElementById(`services-popover-${customerId}`)?.classList.remove('show');
}

function toggleCustomerServiceChip(btn) {
    btn.classList.toggle('selected');
}

function updateCustomerServiceInputs(customerId, form) {
    const container = document.getElementById(`rowSelectedServiceIds-${customerId}`);
    container.innerHTML = '';

    form.querySelectorAll('.service-picker-chip.selected').forEach(chip => {
        const input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'SelectedServiceIds';
        input.value = chip.dataset.value;
        container.appendChild(input);
    });
}

async function submitCustomerServices(e, customerId) {
    e.preventDefault();
    e.stopPropagation();

    const form = e.target;
    updateCustomerServiceInputs(customerId, form);

    const res = await fetch('/Customer/UpdateServices', {
        method: 'POST',
        headers: { 'RequestVerificationToken': getToken() },
        body: new FormData(form),
    });

    const data = await res.json();
    showToast(data.message, data.success ? 'success' : 'error');
    if (data.success) setTimeout(() => location.reload(), 800);
}

// ── Cambiar status ────────────────────────────────────────────
async function changeStatus(id, status) {
    closeAllDropdowns();
    const token = getToken();

    const res = await fetch(`/Customer/ChangeStatus`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token,
        },
        body: `id=${id}&status=${status}`,
    });

    const data = await res.json();
    showToast(data.message, data.success ? 'success' : 'error');
    if (data.success) setTimeout(() => location.reload(), 800);
}


// ── Modal helpers ─────────────────────────────────────────────
function openModal(id) {
    document.getElementById(id)?.classList.add('show');
}

function closeModal(id) {
    document.getElementById(id)?.classList.remove('show');
}

function handleModalClick(e) {
    if (e.target === e.currentTarget) closeModal(e.currentTarget.id);
}

// ── Chips: Representante (selección única) ─────────────────────
function selectRepresentative(btn) {
    document.querySelectorAll('#representativeChips .chip')
        .forEach(c => c.classList.remove('selected'));
    btn.classList.toggle('selected');
    document.getElementById('assignedTo').value =
        btn.classList.contains('selected') ? btn.dataset.value : '';
}

// ── Chips: Servicios (selección múltiple) ─────────────────────
function toggleServiceChip(btn) {
    btn.classList.toggle('selected');
    updateServiceInputs();
}

function updateServiceInputs() {
    const container = document.getElementById('selectedServiceIds');
    container.innerHTML = '';
    document.querySelectorAll('#serviceChips .chip.selected').forEach(chip => {
        const input = document.createElement('input');
        input.type = 'hidden';
        input.name = 'SelectedServiceIds';
        input.value = chip.dataset.value;
        container.appendChild(input);
    });
}

// ── Abrir modal de CREAR ──────────────────────────────────────
function openCreateModal() {
    currentMode = 'create';
    document.getElementById('modalTitle').textContent = 'Añadir nuevo cliente';
    document.getElementById('modalSubmitBtn').textContent = 'Añadir cliente';
    document.getElementById('customerForm').reset();
    document.getElementById('customerId').value = '0';
    document.getElementById('selectedServiceIds').innerHTML = '';

    // Limpiar chips
    document.querySelectorAll('#serviceChips .chip, #representativeChips .chip')
        .forEach(c => c.classList.remove('selected'));

    openModal('customerModal');
}

// ── Abrir modal de EDITAR ─────────────────────────────────────
async function openEditModal(id) {
    currentMode = 'edit';
    document.getElementById('modalTitle').textContent = 'Editar cliente';
    document.getElementById('modalSubmitBtn').textContent = 'Guardar cambios';

    const res = await fetch(`/Customer/EditCustomer?id=${id}`);
    const data = await res.json();

    if (!data.success) { showToast('No se pudo cargar el cliente.', 'error'); return; }

    const c = data.customer;
    document.getElementById('customerId').value = c.id;
    document.getElementById('customerName').value = c.name;
    document.getElementById('customerContact').value = c.contact ?? '';
    document.getElementById('customerEmail').value = c.email ?? '';
    document.getElementById('customerStatus').value = c.status;
    document.getElementById('startDate').value = c.projectStartDate?.substring(0, 10) ?? '';
    document.getElementById('endDate').value = c.projectEndDate?.substring(0, 10) ?? '';
    document.getElementById('customerBudget').value = c.budget ?? '';
    document.getElementById('customerNotes').value = c.notes ?? '';
    document.getElementById('assignedTo').value = c.assignedTo ?? '';

    // Chips de representante
    document.querySelectorAll('#representativeChips .chip').forEach(chip => {
        chip.classList.toggle('selected', chip.dataset.value === c.assignedTo);
    });

    // Chips de servicios
    const selectedIds = c.selectedServiceIds ?? [];
    document.querySelectorAll('#serviceChips .chip').forEach(chip => {
        chip.classList.toggle('selected', selectedIds.includes(parseInt(chip.dataset.value)));
    });
    updateServiceInputs();

    openModal('customerModal');
}

// ── Submit del formulario ─────────────────────────────────────
document.getElementById('customerForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const url = currentMode === 'create' ? '/Customer/Create' : '/Customer/Edit';

    const res = await fetch(url, {
        method: 'POST',
        headers: { 'RequestVerificationToken': getToken() },
        body: new FormData(e.target),
    });

    const data = await res.json();
    if (data.success) {
        closeModal('customerModal');
        showToast(data.message, 'success');
        setTimeout(() => location.reload(), 800);
    } else {
        showToast(data.message, 'error');
    }
});

// ── Archivar ──────────────────────────────────────────────────
async function archiveCustomer(id, name) {
    if (!confirm(`¿Archivar a ${name}?`)) return;
    const token = getToken();

    const res = await fetch('/Customer/Archive', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token,
        },
        body: `id=${id}`,
    });

    const data = await res.json();
    showToast(data.message, data.success ? 'success' : 'error');
    if (data.success) setTimeout(() => location.reload(), 800);
}

// ── Confirmar / ejecutar borrado ──────────────────────────────
function confirmDelete(id, name) {
    deleteTargetId = id;
    document.getElementById('deleteCustomerName').textContent = name;
    openModal('deleteModal');
}

async function executeDelete() {
    if (!deleteTargetId) return;
    const token = getToken();

    const res = await fetch('/Customer/Delete', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token,
        },
        body: `id=${deleteTargetId}`,
    });

    const data = await res.json();
    closeModal('deleteModal');
    showToast(data.message, data.success ? 'success' : 'error');
    if (data.success) setTimeout(() => location.reload(), 800);
}

// ── Email ─────────────────────────────────────────────────────
function sendEmail(email) {
    if (!email) { showToast('Este cliente no tiene correo registrado.', 'error'); return; }
    window.location.href = `mailto:${email}`;
}

// ── Detalles (placeholder) ────────────────────────────────────
function openDetailModal(id) {
    // TODO: implementar modal de detalles
    console.log('Detalles del cliente:', id);
}

//document.addEventListener("DOMContentLoaded", function () {
//    var modalElement = document.getElementById('resultModal');
//    if (modalElement) {
//        var myModal = new bootstrap.Modal(modalElement);
//        myModal.show();
//    }
//});
