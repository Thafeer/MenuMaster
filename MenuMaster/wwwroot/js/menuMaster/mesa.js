﻿function Reload(delay = 3000) {
    setTimeout(() => window.location.reload(), delay);
}

function Back(delay = 3000) {
    setTimeout(() => window.location.href = '/Mesa', delay);

}

$("#btnVoltarMesa").click(function () {
    window.location.href = '/Mesa';
});

function showAlert(type, title, message, sucesso = false) {
    SwalAlertNotification(type, title, message);
    if (sucesso === true) {
        Back()
    }
    else {
        Reload();
    }
}

function CadastrarMesa() {
    let numero = $("#numeroMesa").val();

    if (numero == '') {
        return showAlert("warning", "Atenção!", "Numero deve ser preenchido")
    }

    let objeto = {
        Numero: numero
    };

    try {
        let r = $.ajax({
            url: "/Mesa/CriarMesa",
            method: "POST",
            async: false,
            contentType: "application/json",
            data: JSON.stringify(objeto),
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            }
        });

        if (r.status !== 200) {
            let rData = r.responseJSON;
            throw new Error(JSON.stringify(rData));
        }

        return showAlert("success", "Sucesso!", "Mesa cadastrada com sucesso", true);
    } catch (error) {
        let errorData = JSON.parse(error.message);
        let { descricaoErro, tipoAlerta } = errorData;
        return showAlert(tipoAlerta, "Ops!", descricaoErro);
    }
}

function BloquearMesa(id) {
    if (id == '') {
        return showAlert("warning", "Atenção!", "Identificador da mesa deve ser preenchido")
    }

    Swal.fire({
        title: "Deseja realmente bloquear essa mesa?",
        showDenyButton: true,
        showCancelButton: true,
        cancelButtonText: "Cancelar",
        denyButtonText: `Bloquear`,
        showConfirmButton: false // Oculta o botão "OK"
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isDenied) {
            try {
                let r = $.ajax({
                    url: "/Mesa/BloquearMesa/" + id,
                    method: "GET",
                    async: false,
                    contentType: "application/json",

                });

                if (r.status !== 200) {
                    let rData = r.responseJSON;
                    throw new Error(JSON.stringify(rData));
                }

                return showAlert("success", "Sucesso!", "Mesa bloqueada com sucesso", true);
            } catch (error) {
                let errorData = JSON.parse(error.message);
                let { descricaoErro, tipoAlerta } = errorData;
                return showAlert(tipoAlerta, "Ops!", descricaoErro);
            }
        }

    });
}

function ReativarMesa(id) {
    if (id == '') {
        return showAlert("warning", "Atenção!", "Identificador da mesa deve ser preenchido")
    }

    Swal.fire({
        title: "Deseja realmente reativar essa mesa?",
        showDenyButton: false,
        showCancelButton: true,
        cancelButtonText: "Cancelar",
        confirmButtonText: `Liberar`,
        showConfirmButton: true // Oculta o botão "OK"
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            try {
                let r = $.ajax({
                    url: "/Mesa/ReativarMesa/" + id,
                    method: "GET",
                    async: false,
                    contentType: "application/json",

                });

                if (r.status !== 200) {
                    let rData = r.responseJSON;
                    throw new Error(JSON.stringify(rData));
                }

                return showAlert("success", "Sucesso!", "Mesa liberada com sucesso", true);
            } catch (error) {
                let errorData = JSON.parse(error.message);
                let { descricaoErro, tipoAlerta } = errorData;
                return showAlert(tipoAlerta, "Ops!", descricaoErro);
            }
        }

    });
}