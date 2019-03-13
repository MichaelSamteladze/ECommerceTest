var ContactModel = {
    USCountryID: null,
    CanadaCountryID: null
};
$(function () {
    $('#CountryCombo').on('change', function () {
        $('#StatesCombo select').val([]);
        if ($(this).val() == ContactModel.USCountryID) {
            $('.UsState').show();
            $('.CanadianProvinces').hide();
            $('#StatesCombo').show();
        }
        else if ($(this).val() == ContactModel.CanadaCountryID){
            $('.UsState').hide();
            $('.CanadianProvinces').show();
            $('#StatesCombo').show();
        } else {
            $('#StatesCombo').hide();
        }
    })

    $('form').submit(function () {
        preloader.show();
    })
})