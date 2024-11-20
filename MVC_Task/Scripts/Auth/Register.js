bindEyePassword(".eye-password-btn", ".password-box")
bindEyePassword(".eye-cpassword-btn", ".cpassword-box")
bindEyePassword(".eye-opassword-btn", ".opassword-box")

// Password Strength Checker with 
$(".password-box").on("keyup", function (e) {
    let value = $(this).val()
    var passwordbar = $("#password-bar")

    if (value.length == 0) {
        passwordbar.html("")
        return;
    }

    //Regular Expressions.
    var regex = new Array();
    regex.push("[A-Z]"); //Uppercase Alphabet.
    regex.push("[a-z]"); //Lowercase Alphabet.
    regex.push("[0-9]"); //Digit.
    regex.push("[$@$!%*#?&]"); //Special Character.

    var passed = 0;

    if (value.length >= 8) {
        //Validate for each Regular Expression.
        for (var i = 0; i < regex.length; i++) {
            if (new RegExp(regex[i]).test(value)) {
                passed++;
            }
        }
    }



    //Display status.
    var strength = "";
    switch (passed) {
        case 0:
        case 1:
        case 2:
            strength = $("<small>").addClass("progress-bar progress-bar-danger").css("width", "40%").text("Weak")
            break;
        case 3:
            strength = $("<small>").addClass("progress-bar progress-bar-warning").css("width", "60%").text("Medium");
            break;
        case 4:
            strength = $("<small>").addClass("progress-bar progress-bar-success").css("width", "100%").text("Strong");
            break;

    }
    passwordbar.html(strength)

})
