$(document).ready(function () {
    $('.telefone').mask('(00) 00000-0000');

    $('.dinheiro').mask('000.000.000.000.000,00', { reverse: true });
});

function RemoverMascara(valor) {
    let valorSemPontos = valor.replace(/\./g, '');
    let valorFormatado = valorSemPontos.replace(',', '.');
    return valorFormatado;
}

function SwalAlertNotification(icon, title, message) {

    let notification = {
        icon: icon,
        title: title,
        message: message
    };

    SwalAlert(notification);
    function SwalAlert(options) {
        // Padrão:
        // SwalAlert({ icon: "success" - "warning" - "danger", title: "titulo notificacao", text: "texto mensagem" });
        Swal.fire({
            icon: options.icon,
            title: options.title,
            text: options.message,
            showConfirmButton: true,
            timer: 5000,
            timerProgressBar: true,
        });
    }
}