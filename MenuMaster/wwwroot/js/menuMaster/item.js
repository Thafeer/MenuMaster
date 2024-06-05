function Reload(delay = 3000) {
    setTimeout(() => window.location.reload(), delay);
}

function Back(delay = 3000) {
    setTimeout(() => window.location.href = '/MenuItens', delay);

}

$("#btnVoltarItem").click(function () {
    window.location.href = '/MenuItens';
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

function CadastrarItem() {
    let nome = $("#nomeItem").val();
    let preco = $("#valor").val();
    let tipo = $("#tipoItem").val();
    let disponibilidadeSelecionada = $("#disponibilidade").val();

    if (nome == '' || preco == '' || tipo == '' || disponibilidadeSelecionada == '') {
        return showAlert("warning", "Atenção!", "Nome, Preço, Tipo e Disponibilidade devem ser preenchidos")
    }

    let objeto = {
        Nome: nome,
        Preco: parseFloat(RemoverMascara(preco)),
        Tipo: tipo,
        Disponivel: disponibilidadeSelecionada === "1" ? true : false
    };

    try {
        let r = $.ajax({
            url: "/MenuItens/CriarItem",
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

        return showAlert("success", "Sucesso!", "Item cadastrado com sucesso", true);
    } catch (error) {
        let errorData = JSON.parse(error.message);
        let { descricaoErro, tipoAlerta } = errorData;
        return showAlert(tipoAlerta, "Ops!", descricaoErro);
    }  
}

function AlterarItem() {
    let altPreco = $("#altValor").val();
    let altId = $("#altId").val();
    let altDisponibilidade = $("#altDisponibilidade").val();

    if (altPreco == '' || altDisponibilidade == '') {
        return showAlert("warning", "Atenção!", "Preço e Disponibilidade devem ser preenchidos")
    }

    let altObjeto = {
        Id: altId,
        Preco: parseFloat(RemoverMascara(altPreco)),
        Disponivel: altDisponibilidade === "1" ? true : false
    };

    try {
        let r = $.ajax({
            url: "/MenuItens/EditarItem",
            method: "POST",
            async: false,
            contentType: "application/json",
            data: JSON.stringify(altObjeto),
            headers: {
                "RequestVerificationToken": $('input[name="__RequestVerificationToken"]').val()
            }
        });

        if (r.status !== 200) {
            let rData = r.responseJSON;
            throw new Error(JSON.stringify(rData));
        }

        return showAlert("success", "Sucesso!", "Item alterado com sucesso", true);
    } catch (error) {
        let errorData = JSON.parse(error.message);
        let { descricaoErro, tipoAlerta } = errorData;
        return showAlert(tipoAlerta, "Ops!", descricaoErro);
    }
}