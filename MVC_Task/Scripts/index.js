/**
 * Used for the functionality of the eye button of password.
 * @param {string} passwordbtn
 * @param {string} passwordinput
 */
function bindEyePassword(passwordbtn, passwordinput) {
    $(passwordbtn).click(function () {
        var passwordbox = $(passwordinput)
        var type = passwordbox.attr("type") == "text" ? "password" : "text"
        if (type == "text") {
            $(this).removeClass("glyphicon-eye-open")
            $(this).addClass("glyphicon-eye-close")
        }
        else {
            $(this).addClass("glyphicon-eye-open")
            $(this).removeClass("glyphicon-eye-close")
        }
        passwordbox.attr("type", type)
    })
}


/**
 * Used to fadeout the alert box of bootstrap after 3 seconds
 * 
 */
$(function () {
    setTimeout(function () {
        $(".alert").fadeOut(1000);
    }, 3000)
})

/**
 * Function to get the value of the cookie of the specified key
 * @param {string} name
 * @returns Value of the specified key. if not found then returns null
 */
function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

// Setting the icons to the header link of the dropdown
$(".profile-header-link").prepend($("<span>").addClass("glyphicon glyphicon-edit padding-right-8"))
$(".setting-header-link").prepend($("<span>").addClass("glyphicon glyphicon-cog padding-right-8"))

// Setting the Icons to the pagination 
$(".prev-page-btn>*").prepend($("<span>").addClass("glyphicon glyphicon-arrow-left padding-right-8"))
$(".next-page-btn>*").append($("<span>").addClass("glyphicon glyphicon-arrow-right padding-left-8"))