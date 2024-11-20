$(function () {

    /* Js Code for payment of fine */
    $(".return-book-btn").click(function () {
        $("#Modal-Payment").modal({
            keyboard: false,
            backdrop: 'static'
        });

        $("#Modal-Payment .modal-yes-btn").click(function () {
            $("#return-book-form").submit();
        });
    })

})