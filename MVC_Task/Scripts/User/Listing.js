$(function () {
    let username = "";
    let userid = 0;

    $(".list-filter").on("change", function () {
        $(".filter-form").submit();
    });

    $(".delete-btn").click(function () {
        username = $(this).data("username");
        userid = $(this).data("userid");
        $(".modal #userid").text(userid);
        $(".modal #username").text(username);
        $("#myModal").modal({
            keyboard: false,
            backdrop: "static"
        });
    });

    $(".listing-modal.modal .modal-yes-btn").click(function () {
        $(this).html("Wait.....");
        $.ajax({
            method: "POST",
            url: `/User/Delete/${userid}`,
            complete: function (d) {
                console.log(d, "completed")
            },
            error: function (d) {
                console.log(d)
                window.location.reload()
            },
            success: function (d) {
                if (d?.status === "Success") {
                    window.location.reload()
                }
                else {
                    window.location.reload()
                }
            }
        });
    });
});