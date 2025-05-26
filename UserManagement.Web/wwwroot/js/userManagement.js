document.addEventListener('DOMContentLoaded', function () {

    let isProcessing = false;

    const selectAllCheckbox = document.getElementById('selectAll');
    const userCheckboxes = document.querySelectorAll('.user-checkbox');
    const blockBtn = document.getElementById('blockBtn');
    const unblockBtn = document.getElementById('unblockBtn');
    const deleteBtn = document.getElementById('deleteBtn');

    selectAllCheckbox.addEventListener('change', function () {
        const isChecked = this.checked;
        userCheckboxes.forEach(checkbox => {
            checkbox.checked = isChecked;
        });
        updateButtonStates();
    });

    userCheckboxes.forEach(checkbox => {
        checkbox.addEventListener('change', function () {
            updateSelectAllState();
            updateButtonStates();
        });
    });

    function updateSelectAllState() {
        const totalCheckboxes = userCheckboxes.length;
        const checkedCheckboxes = Array.from(userCheckboxes).filter(cb => cb.checked).length;

        selectAllCheckbox.checked = checkedCheckboxes === totalCheckboxes;
        selectAllCheckbox.indeterminate = checkedCheckboxes > 0 && checkedCheckboxes < totalCheckboxes;
    }

    function updateButtonStates() {
        const hasSelection = Array.from(userCheckboxes).filter(cb => cb.checked).length > 0;

        blockBtn.disabled = !hasSelection;
        unblockBtn.disabled = !hasSelection;
        deleteBtn.disabled = !hasSelection;
    }

    function getSelectedUserIds() {
        return Array.from(userCheckboxes)
            .filter(cb => cb.checked)
            .map(cb => parseInt(cb.value));
    }

    blockBtn.addEventListener('click', function () {
        if (isProcessing) return;

        const selectedIds = getSelectedUserIds();
        if (selectedIds.length === 0) return;

        isProcessing = true;

        performAjaxAction('/User/Block', selectedIds, 'blocking')
            .finally(() => isProcessing = false);;
    });

    unblockBtn.addEventListener('click', function () {
        if (isProcessing) return;

        const selectedIds = getSelectedUserIds();
        if (selectedIds.length === 0) return;

        isProcessing = true;

        performAjaxAction('/User/Unblock', selectedIds, 'unblocking')
            .finally(() => isProcessing = false);;
    });

    deleteBtn.addEventListener('click', function () {
        if (isProcessing) return;

        const selectedIds = getSelectedUserIds();
        if (selectedIds.length === 0) return;

        isProcessing = true;

        performAjaxAction('/User/Delete', selectedIds, 'deleting')
            .finally(() => isProcessing = false);;
    });

    function performAjaxAction(url, userIds, actionName) {
        fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(userIds)
        })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    toastr.success(data.message);

                    if (data.redirectToLogin) {
                        setTimeout(function () {
                            window.location.href = '/Auth/Login';
                        }, 2000);
                    } else {
                        location.reload();
                    }
                } else {
                    toastr.error(data.message || `Error ${actionName} users`);
                }
            })
            .catch(error => {
                toastr.error(`Error ${actionName} users. Please try again.`);
            });
    }
});