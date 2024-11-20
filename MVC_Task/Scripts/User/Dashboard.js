$(function () {
    $(".lib-return-btn").click(function () {
        $("#booktitle").html($(this).data("title"))
        $("#issuername").html($(this).data("issuername"))

        $("#Return-Modal-Payment").modal({
            keyboard: false,
            backdrop: "static"
        })

        $("#Return-Modal-Payment .modal-yes-btn").click(function () {
            $("#library-return-book-form").submit()
        })
    })
})