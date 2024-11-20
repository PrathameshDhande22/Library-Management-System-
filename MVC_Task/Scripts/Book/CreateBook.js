$(document).ready(function () {

    function showAlert(className, errormsg) {
        $(".modal-yes-btn").html("Add").attr("disabled", false)
        $(className).show();
        if (errormsg !== "Success") {
            $(className).text(errormsg)
        }

        setTimeout(function () {
            $(className).fadeOut(1000);
        }, 3000);
    }

    /* JS Code for Category addition  */
    $(".category-add-btn").click(function () {
        $("#modal-display").modal({
            keyboard: false,
            backdrop: "static"
        })
        $("#category-name").focus();
    })



    // * Automatic setting the value of dropdown box
    var categorydropdown = $("#Category_CategoryId")
    if (categorydropdown.data("selected") !== "") {
        var categories = categorydropdown.find(`option[value=${categorydropdown.data("selected")}]`).attr("selected", true)
    }


    function createCategory(categoryname) {
        $(".modal #category-name").val("")
        if (categoryname === "" || categoryname === null) {
            $(".modal-body .category-name-error").html("Category Name Required").addClass("text-danger")
            $(".modal #category-name").addClass("input-validation-error")
        } else {
            $(".modal-yes-btn").html("Adding....").attr("disabled", true)
            // checking if the category already present in the option
            let values = Array.from($("#Category_CategoryId > option")).filter((value, index) => {
                return $(value).text() === categoryname
            })
            if (values.length >= 1) {
                showAlert(".creation-error", "Category Already Present")
            } else {
                $.ajax({
                    method: "POST",
                    url: `/Book/CategoryAdd`,
                    data: { "category": categoryname },
                    complete: function (d) {
                        $(".modal-yes-btn").html("Add").attr("disabled", false)
                    },
                    success: function (d) {

                        $("#modal-display").modal("hide")
                        if (d?.status === "Success") {
                            showAlert(".creation-successful", "Success")
                            $("#Category_CategoryId").append($("<option>").html(categoryname).attr({
                                value: d?.insertedid,
                                selected: true
                            })).removeClass("input-validation-error")
                            $("#Category_CategoryId-error").remove()
                        } else {
                            showAlert(".creation-error", d.message)
                        }
                    },
                    error: function (d) {
                        console.log(d)
                    }
                })
            }

        }
    }

    function handleModal() {
        let categoryname = $(".modal #category-name").val()
        createCategory(categoryname)
        if (categoryname !== "") {
            $("#modal-display").modal("hide")
        }
    }

    $("#modal-display").on("keyup", function (e) {
        if (e.key === "Enter") {
            handleModal()
        }
    })

    $("#modal-display .modal-yes-btn").click(function () {
        handleModal()
    })

    // when the close button is clicked clear all the errors in the modal
    $("#modal-display .btn-modal-close,#modal-display .modal-header .close").click(function () {
        $(".modal-body .category-name-error").html("")
        $(".modal #category-name").removeClass("input-validation-error")
    })


    // Reset Button Functionality to remove the selected Category
    $(".book-form .reset-btn").click(function () {
        $("#Category_CategoryId").find("option[selected='selected']").attr("selected", false)
    })

    //* JS Code for throwing the error if the image uploaded is wrong format *//
    // if image selected then preview the image

    const supportedimages = ["image/png", "image/jpeg", "image/jpg"];

    // for handling the images uploaded
    $("#CoverImageFile").on("change", function (e) {
        $(this).removeClass("input-validation-error")
        let typeuploaded = e.target.files[0]?.type

        var imagepreview = $(".imageshow.image-preview")

        $(".previewimageshow").remove();
        $(".coverimageshow").hide();
        $(".tempimageshow").hide();


        var buttontag = $("<button>").addClass("btn btn-default margin-left-right-15 remove-image-btn").html("<span class='glyphicon glyphicon-trash padding-right-4'> </span> Remove").attr("type", "button").click(function () {
            $("#CoverImageFile")[0].value = null;
            $(".previewimageshow").remove()
            if (imagepreview.children(".tempimageshow")[0]) {
                $(".tempimageshow").show();
            }
            else if (imagepreview.children(".coverimageshow")[0]) {
                $(".coverimageshow").show();
            }
        })

        var divtag = $("<div>").append($("<span>").text("Preview").addClass("d-block font-bold")).addClass("previewimageshow")

        if (supportedimages.indexOf(typeuploaded) === -1) {
            var infotag = $("<span>").html("The Uploaded File is Not Supported and will not be Uploaded").addClass("text-danger font-bold")
            $("#CoverImageFile").addClass("input-validation-error")
            divtag.append(infotag)
        } else {
            var image = URL.createObjectURL(e.target.files[0])
            var imagetag = $("<img>").attr("src", image)
            divtag.append(imagetag)
        }

        divtag.append(buttontag)
        imagepreview.append(divtag)
    })

    $(".edit-btn").append($("<span>").addClass("glyphicon glyphicon-edit padding-left-4"))



    /* JS Code for Quantity increasing and decreasing logic */

    var quantitybook = $("#quantity-book")
    var incrementbtn = $(".btn-increment-quantity")
    var decrementbtn = $(".btn-decrement-quantity")

    if (Number(quantitybook.val()) <= 0) {
        decrementbtn.attr("disabled", true)
    }

    function enableDisableDecrementBtn(value) {
        if (value <= 0) {
            decrementbtn.attr("disabled", true)
        } else {
            decrementbtn.attr("disabled", false)
        }
    }

    quantitybook.on("change", function () {
        let value = $(this).val()
        enableDisableDecrementBtn(value)
    })

    incrementbtn.click(function () {
        let newvalue = Number(quantitybook.val())
        enableDisableDecrementBtn(newvalue)
        quantitybook.val(Number(quantitybook.val()) + Number(1))
    })

    decrementbtn.click(function () {
        let oldvalue = Number(quantitybook.val())
        quantitybook.val(oldvalue - Number(1))
        let newvalue = Number(quantitybook.val())
        enableDisableDecrementBtn(newvalue)
    })

    /* Js Code to delete book */
    let username = "";
    let userid = "";
    $(".delete-btn").click(function () {
        username = $(this).data("username")
        userid = $(this).data("userid")
        $(".book-listing-modal.modal #userid").text(userid)
        $(".book-listing-modal.modal #username").text(username)
        $("#BookModal").modal({
            keyboard: false,
            backdrop: "static"
        })
    })

    $(".book-listing-modal .modal-yes-btn").click(function () {
        $(this).html("Adding....").attr("disabled", true)
        window.scrollTo(0, 0)
        $.ajax({
            method: "POST",
            url: `/Book/DeleteBook/${userid}`,
            complete: function (d) {
                $(".book-listing-modal .modal-yes-btn").html("Add").attr("disabled", false)
            },
            success: function (d) {
                $("#BookModal").modal("hide")
                if (d?.status === "success") {
                    window.location.reload()
                }
                window.location.reload()
            },
            error: function (d) {
                window.location.reload()
            }
        })
    })
})