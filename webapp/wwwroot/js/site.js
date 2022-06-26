// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready( function() {
    var title = document.title;
    alert(title);
    if (title == 'Nicholas R Johnson - Home')
    {
        $("#homeIdTop").css("color", "#76a3fb"); 
        $("#aboutIdTop").css("color", "black"); 
        $("#projectsIdTop").css("color", "black"); 
        $("#contactIdTop").css("color", "black"); 
        $("#innerIdTop").css("color", "black");
    }

    if (title == 'Nicholas R Johnson - About')
    {
        $("#homeIdTop").css("color", "black"); 
        $("#aboutIdTop").css("color", "#76a3fb"); 
        $("#projectsIdTop").css("color", "black"); 
        $("#contactIdTop").css("color", "black"); 
        $("#innerIdTop").css("color", "black");
    }

    if (title == 'Nicholas R Johnson - Contact')
    {
        $("#homeIdTop").css("color", "black"); 
        $("#aboutIdTop").css("color", "black"); 
        $("#projectsIdTop").css("color", "black"); 
        $("#contactIdTop").css("color", "#76a3fb"); 
        $("#innerIdTop").css("color", "black");
    }

    if (title == 'Nicholas R Johnson - Projects')
    {
        $(document).ready( function() {
            $("#homeIdTop").css("color", "black"); 
            $("#aboutIdTop").css("color", "black"); 
            $("#projectsIdTop").css("color", "#76a3fb"); 
            $("#contactIdTop").css("color", "black"); 
            $("#innerIdTop").css("color", "black");
        });
    }

    if (title == 'Nicholas R Johnson - Welcome In')
    {
        $("#homeIdTop").click(function() {
            $("#homeIdTop").css("color", "#76a3fb"); 
            $("#aboutIdTop").css("color", "black"); 
            $("#projectsIdTop").css("color", "black"); 
            $("#contactIdTop").css("color", "black"); 
            $("#insideIdTop").css("color", "black");
        });
        $("#aboutIdTop").click(function() {
            $("#homeIdTop").css("color", "black"); 
            $("#aboutIdTop").css("color", "#76a3fb"); 
            $("#projectsIdTop").css("color", "black"); 
            $("#contactIdTop").css("color", "black"); 
            $("#insideIdTop").css("color", "black");
        });
        $("#projectsIdTop").click(function() {
            $("#homeIdTop").css("color", "black"); 
            $("#aboutIdTop").css("color", "black"); 
            $("#projectsIdTop").css("color", "#76a3fb"); 
            $("#contactIdTop").css("color", "black"); 
            $("#insideIdTop").css("color", "black");
        });
        $("#contactIdTop").click(function() {
            $("#homeIdTop").css("color", "black"); 
            $("#aboutIdTop").css("color", "black"); 
            $("#projectsIdTop").css("color", "black"); 
            $("#contactIdTop").css("color", "#76a3fb"); 
            $("#insideIdTop").css("color", "black");
        });
        $("#insideIdTop").click(function() {
            $("#homeIdTop").css("color", "black"); 
            $("#aboutIdTop").css("color", "black"); 
            $("#projectsIdTop").css("color", "black"); 
            $("#contactIdTop").css("color", "black"); 
            $("#insideIdTop").css("color", "#76a3fb");
        });
    }

    if (title == 'Nicholas R Johnson - Admin')
    {
        $(document).ready( function() {
            $("#settingsMenu").live( "click", "#adminProfile", function() {
                $('#adminProfile').addClass('active');
                $('#adminChangeEmail').removeClass('active');
                $('#adminChangePassword').removeClass('active');
                $('#adminExternalLogins').removeClass('active');
                $('#adminTwoFactorAuthentication').removeClass('active');
                $('#adminPersonalData').removeClass('active');
            });

            $("#settingsMenu").live( "click", "#adminChangeEmail", function() {
                $('#adminProfile').removeClass('active');
                $('#adminChangeEmail').addClass('active');
                $('#adminChangePassword').removeClass('active');
                $('#adminExternalLogins').removeClass('active');
                $('#adminTwoFactorAuthentication').removeClass('active');
                $('#adminPersonalData').removeClass('active');
            });
            $("#settingsMenu").live( "click", "#adminChangePassword", function() {
                $('#adminProfile').removeClass('active');
                $('#adminChangeEmail').removeClass('active');
                $('#adminChangePassword').addClass('active');
                $('#adminExternalLogins').removeClass('active');
                $('#adminTwoFactorAuthentication').removeClass('active');
                $('#adminPersonalData').removeClass('active');
            });
            $("#settingsMenu").live( "click", "#adminExternalLogins", function () {
                $('#adminProfile').removeClass('active');
                $('#adminChangeEmail').removeClass('active');
                $('#adminChangePassword').removeClass('active');
                $('#adminExternalLogins').addClass('active');
                $('#adminTwoFactorAuthentication').removeClass('active');
                $('#adminPersonalData').removeClass('active');
            });
            $("#settingsMenu").live( "click", "#adminTwoFactorAuthentication", function() {
                $('#adminProfile').removeClass('active');
                $('#adminChangeEmail').removeClass('active');
                $('#adminChangePassword').removeClass('active');
                $('#adminExternalLogins').removeClass('active');
                $('#adminTwoFactorAuthentication').addClass('active');
                $('#adminPersonalData').removeClass('active');
            });
            $("#settingsMenu").live( "click", "@adminPersonalData", function() {
                $('#adminProfile').removeClass('active');
                $('#adminChangeEmail').removeClass('active');
                $('#adminChangePassword').removeClass('active');
                $('#adminExternalLogins').removeClass('active');
                $('#adminTwoFactorAuthentication').removeClass('active');
                $('#adminPersonalData').addClass('active');
            });
        });
    } 
});