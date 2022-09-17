// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready( function() {
    var title = document.title;
    alert(title);
    if (title == 'Nicholas R Johnson - Home')
    {
        $("#homeIdTop").css("color", "#efad32"); 
        $("#aboutIdTop").css("color", "#b27c17"); 
        $("#projectsIdTop").css("color", "#b27c17"); 
        $("#contactIdTop").css("color", "#b27c17"); 
        $("#innerIdTop").css("color", "#b27c17");
    }

    if (title == 'Nicholas R Johnson - About')
    {
        $("#homeIdTop").css("color", "#b27c17"); 
        $("#aboutIdTop").css("color", "#efad32"); 
        $("#projectsIdTop").css("color", "#b27c17"); 
        $("#contactIdTop").css("color", "#b27c17"); 
        $("#innerIdTop").css("color", "#b27c17");
    }

    if (title == 'Nicholas R Johnson - Contact')
    {
        $("#homeIdTop").css("color", "#b27c17"); 
        $("#aboutIdTop").css("color", "#b27c17"); 
        $("#projectsIdTop").css("color", "#b27c17"); 
        $("#contactIdTop").css("color", "#efad32"); 
        $("#innerIdTop").css("color", "#b27c17");
    }

    if (title == 'Nicholas R Johnson - Projects')
    {
        $(document).ready( function() {
            $("#homeIdTop").css("color", "#b27c17"); 
            $("#aboutIdTop").css("color", "#b27c17"); 
            $("#projectsIdTop").css("color", "#efad32"); 
            $("#contactIdTop").css("color", "#b27c17"); 
            $("#innerIdTop").css("color", "#b27c17");
        });
    }

    if (title == 'Nicholas R Johnson - Welcome In')
    {
        $("#homeIdTop").click(function() {
            $("#homeIdTop").css("color", "#efad32"); 
            $("#aboutIdTop").css("color", "#b27c17"); 
            $("#projectsIdTop").css("color", "#b27c17"); 
            $("#contactIdTop").css("color", "#b27c17"); 
            $("#insideIdTop").css("color", "#b27c17");
        });
        $("#aboutIdTop").click(function() {
            $("#homeIdTop").css("color", "#b27c17"); 
            $("#aboutIdTop").css("color", "#efad32"); 
            $("#projectsIdTop").css("color", "#b27c17"); 
            $("#contactIdTop").css("color", "#b27c17"); 
            $("#insideIdTop").css("color", "#b27c17");
        });
        $("#projectsIdTop").click(function() {
            $("#homeIdTop").css("color", "#b27c17"); 
            $("#aboutIdTop").css("color", "#b27c17"); 
            $("#projectsIdTop").css("color", "#efad32"); 
            $("#contactIdTop").css("color", "#b27c17"); 
            $("#insideIdTop").css("color", "#b27c17");
        });
        $("#contactIdTop").click(function() {
            $("#homeIdTop").css("color", "#b27c17"); 
            $("#aboutIdTop").css("color", "#b27c17"); 
            $("#projectsIdTop").css("color", "#b27c17"); 
            $("#contactIdTop").css("color", "#efad32"); 
            $("#insideIdTop").css("color", "#b27c17");
        });
        $("#insideIdTop").click(function() {
            $("#homeIdTop").css("color", "#b27c17"); 
            $("#aboutIdTop").css("color", "#b27c17"); 
            $("#projectsIdTop").css("color", "#b27c17"); 
            $("#contactIdTop").css("color", "#b27c17"); 
            $("#insideIdTop").css("color", "#efad32");
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