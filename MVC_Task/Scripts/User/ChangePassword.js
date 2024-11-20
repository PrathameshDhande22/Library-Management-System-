
// To check the old password and new password using js
$(function () {
    var oldpasswordbox = $(".opassword-box")
    var newpasswordbox = $(".password-box")

    var passwordmsg = $(".password-msg")

    function ComparePassword() {
        if (oldpasswordbox.val()) {
            if (oldpasswordbox.val() === newpasswordbox.val()) {
                passwordmsg.find("#npassword-error").remove();
                passwordmsg.append($("<span>").attr("id", "npassword-error").addClass("d-block").html("New Password Must be different from Old Password"))
            }
        }
    }

    newpasswordbox.on("change keyup blur", ComparePassword)
    oldpasswordbox.on("change keyup blur", ComparePassword)
})