function blazorOpenModal(dialog) {
    if (!dialog.open) {
        dialog.showModal();
    }
}

function blazorCloseModal(dialog) {
    if (dialog.open) {
        dialog.close();
    }
}

window.scrollToElement = function () {
    let element = document.getElementById('componentHistorico');

    element.scrollIntoView({ behavior: 'smooth', block: 'start' });
}