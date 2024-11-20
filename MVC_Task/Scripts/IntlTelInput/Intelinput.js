// Intelinput starter
$(function () {
    const input = document.querySelector("#phone");
    const intl = window.intlTelInput(input, {
        initialCountry: "in",
        separateDialCode: true,

    });

    const errorMap = ["Invalid number", "Invalid country code", "Too short", "Too long", "Invalid number"];

    var validationmsg = $(".phone-msg")
    function showError(msg) {
        removeError()
        validationmsg.append($("<span>").attr("id", "phone-error").html(msg))
    }

    function removeError() {
        validationmsg.find("#phone-error").remove()
    }

    function verifyPhoneNo() {
        if (intl.isValidNumberPrecise()) {
            removeError();
            return true;
        } else {
            const errorcode = intl.getValidationError()
            if (intl.getNumber() === '') {
                showError("Phone Number Field Is Required")
                return;
            }
            const errmsg = errorMap[errorcode] || "Invalid Number"
            showError(errmsg)
            return false;
        }
        return true;
    }

    $(".register-form").on("submit", function (e) {
        e.preventDefault();
        if (verifyPhoneNo()) {
            console.log("turned off")
            $(".register-form").off().submit()
        }


    })

    $("#phone").on({
        keyup: verifyPhoneNo,
        change: verifyPhoneNo,
        blur: verifyPhoneNo
    })
})