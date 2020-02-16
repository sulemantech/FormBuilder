var global = function () {

    function bindGlobals() {

        // bind lightboxes
        $U.bindLightboxes();

        // bind the tooltip items
        $U.bindTipsies();

        // preload images using image paths loaded in a span with class
        // "preaload-path"
        $('.preload-path').each(function (index, item) {
            var img = new Image();
            img.src = $(item).text();
        });

        // bind watermarked input fields
        $U.bindWatermarks();

        // bind elastics
        $U.bindElastics();

        // bind link submits
        $U.bindLinkButtonSubmit();

        // bind characters-left counter
        $U.bindTextAreaMax();

        // bind popup windows
        $U.bindPopups();

        // ajax action links
        $U.bindAjaxActionLinks();

        // bind redirect buttongs
        $U.bindRedirectButton();

        // bind More/less
        $U.bindMoreLess();

        // prevent enter submit on some fields
        $U.preventEnterSubmit();

        //submits checkbox form after checking
        $U.bindCheckboxSubmit();

        // prevent double form submit
        $U.preventDoubleFormSubmit();

        // restrict numeric input elements
        $U.bindNumerics();



        $('input.select').live('click', function () {
            $(this).select();
        });

        $('input.formatted-currency-field').live('blur', function () {
            try {
                if ($(this).val().length > 0) {
                    $(this).val(parseFloat($(this).val()).toFixed(2));
                }
            } catch (e) {

            }
        });
    }

    function getRoot() {
        return location.protocol + '//' + location.host
    }

    function setEqualHeight() {
        columns = $("div.equal-height-column");
        var tallestcolumn = 0;
        columns.each(
         function () {
             currentHeight = $(this).height();
             if (currentHeight > tallestcolumn) {
                 tallestcolumn = currentHeight;
             }
         }
        );
        columns.css('min-height', tallestcolumn);
    }


    function getItemId(item) {
        return item.attr('data-item-id');
    }


    function modalAjaxCallback() {
        $U.log('modal callback')
        if ($('#is-modal-ajax-succesful')) {
            if ($('#is-modal-ajax-succesful').val() == "true") {
                var message = $('#modal-ajax-message').val();
                $U.writeMessageAndCloseLightbox(message);
            }
        }
    }


    function init() {
        bindGlobals();
        setEqualHeight();
    }

    return {
        init: init,
        getRoot: getRoot,
        getItemId: getItemId,
        modalAjaxCallback: modalAjaxCallback
    }

} ();


try {
    // do not show error if this fails, not mission-critical
    $.validator.addMethod("zipcode", function (postalcode, element) {
        //removes placeholder from string
        postalcode = postalcode.split("_").join("");

        //Checks the length of the zipcode now that placeholder characters are removed.
        if (postalcode.length === 6) {
            //Removes hyphen
            postalcode = postalcode.replace("-", "");
        }
        //validates postalcode.
        return this.optional(element) || postalcode.match(/^\d{5}$|^\d{5}\-\d{4}$/);
    }, "Please specify a valid zip code");

} catch (ex) {

}





