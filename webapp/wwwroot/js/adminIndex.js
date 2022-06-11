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