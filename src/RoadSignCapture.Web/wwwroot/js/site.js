// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $("#userMenuBtn").on("click", function (e) {
        e.preventDefault();
        $("#userMenu").toggleClass("hidden");
    });

    // Close menu if clicked outside
    $(document).on("click", function (e) {
        if (!$(e.target).closest("#userMenuBtn, #userMenu").length) {
            $("#userMenu").addClass("hidden");
        }
    });
});
