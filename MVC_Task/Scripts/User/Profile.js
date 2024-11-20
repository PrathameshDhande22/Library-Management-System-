$(function () {


    /**
     * Function to Disable all inputs present in the current Page
     * @param {Boolean} value
     */
    function disabledAllInputs(value) {
        $(".profile-edit .form-control").prop("disabled", value)
        $(".profile-edit .radio-inline > input[type=radio]").prop("disabled", value)
    }

    $(".back-btn").hide();


    if (getCookie("iseditable") === "false" || getCookie("iseditable") === null) {
        disabledAllInputs(true)
        $(".profile-edit .edit-btn").on("click", function (e) {
            $(this).attr("type", "submit").removeClass("btn-default edit-btn").addClass("btn-success save-btn").text("Save")
            disabledAllInputs(false)
            e.preventDefault();
            $(".back-btn").show();
            $(this).off();
        })

    } else {
        $(".profile-edit .edit-btn").attr("type", "submit").removeClass("btn-default edit-btn").addClass("btn-success save-btn").text("Save");
        disabledAllInputs(false)
        $(".back-btn").show();
    }
})