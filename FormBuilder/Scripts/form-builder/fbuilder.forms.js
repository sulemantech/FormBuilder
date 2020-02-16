var forms = function () {

    var dropPosition = 0;
    var activeItemId = 0;
    var activeControl = 'form';
    var append = false;
    var inSort = false;
    function bindBasicActions() {

        // bind tools menu togglers
        $('.menu-toggle').click(function () {
            var link = $(this)
            var idParts = link.attr("id").split("-");
            var toToggle = $('#' + idParts[0] + '-' + idParts[1] + '-list');

            if (toToggle.is(':visible')) {
                toToggle.slideUp(function () {
                    link.find('span')
                        .addClass('menu-toggle-down-icon')
                        .removeClass('menu-toggle-up-icon')
                });
            } else {
                toToggle.slideDown(function () {
                    link.find('span')
                        .addClass('menu-toggle-up-icon')
                        .removeClass('menu-toggle-down-icon')
                });
            }
        });

        // bind settings actions
        // TODO: wire-up to ajax delete function to remove from DB
        // delete action
        $('.delete-field-icon').live('click', function () {
            if (confirm("Are you sure you'd like to delete this field? All entries for this field will be lost.")) {
                $('#field-property-container').hide();
                $("#form-property-container").show();
                toggleSaveButton();
                autoDeleteField = false;
                doDeleteField($(this));
            }



        });

        // active action
        $('li.drop-item, .editable').live('click', function () {
            var parentItem = $(this).closest('li.drop-item');
            activateField(parentItem);

        });

        // list item hover
        $('li.drop-item').live({
            mouseenter:
         function () {
             $(this).find('.hidden-icon').removeClass('hide');
         },
            mouseleave:
         function () {
             $(this).find('.hidden-icon').addClass('hide');
         }
        });

        // make form title editable
        $('.form-editable').live('click', function () {
            activeItemId = '0';
            loadProperties();
        })

        // wire up cancel event for upgrade
        $('.cancel-upgrade-link').live('click', closeUpgradeModal);

    } // end basic actions

    function doDeleteField(target) {
        target.closest('li.drop-item').slideUp('fast', function () {

            var domId = $(this).attr('data-dom-id'),
                evtId = $('#EventId').val(),
                fieldId = $(this).find('#fieldid-prop-' + domId).val();

            $(this).remove();
            if (fieldId) {
                $.post('/forms/deletefield', { eventid: evtId, fieldid: fieldId }, function () {
                    refreshForm();
                });
            }
        })
    }

    function closeUpgradeModal() {
        $.modal.close();
        var deleteIcon = $('#drop-form').find('li.unavailable').find('.delete-field-icon');
        doDeleteField(deleteIcon);
    }

    function bindDragAndDrop() {
        $('.form-tool').draggable({
            helper: draggableElement,
            snap: "#drop-form",
            start: startToolDrag

        });

        $('#drop-form').droppable({
            activate: handleActive,
            over: handleOver,
            drop: handleDrop,
            out: handleOut,
            hoverClass: 'drop-form-hover',
            activeClass: 'drop-form-hover'

        });
    }

    function startToolDrag(event, ui) {
        inSort = false;
    }

    function handleOver(event, ui) {
        append = true;
        removeGuide();
    }

    function handleOut(event, ui) {
        removeGuide();
    }

    function handleActive(event, ui) {

    }

    function handleDrop(event, ui) {
        if (!inSort) {
            var controlType = $(ui.draggable).attr('data-type');
            var isAvailable = true;
            if ($(ui.draggable).attr('data-isavailable')) {
                isAvailable = $(ui.draggable).attr('data-isavailable').toLowerCase() == 'true';
            }
            if (ui.offset.left >= 300 || $('li.guido').length > 0) {
                // if this current item being dragged
                var dropPos = getDropPos();
                var newItem = generateItem(controlType, dropPos, isAvailable);


                // remove previously active item
                $('li.drop-item').removeClass('active');
                if ($('.drop-item').length == 0 || append == true) {
                    $('#drop-form').append(newItem);
                } else {
                    var selector = "li.drop-item:eq(" + (dropPos - 1) + ")"
                    $(selector).before(newItem);
                }

                refreshForm();
                $U.highlight(newItem);
                $U.bindWatermarks();
                loadProperties();
                removeGuide();
                bindDroppableItems();
                bindSettingsActions(newItem);
                refreshSortOrder();
                bindEditables();
                toggleSaveButton();
                handleAvailability();


            }
        }
    }

    function handleAvailability() {
        setTimeout(function () {
            var anyUnavaiables = $('#drop-form').find('li.unavailable').length > 0
            // console.log(anyUnavaiables);
            if (anyUnavaiables) {
                autoDeleteField = true;
                $.modal($('#upgrade-required-template').tmpl(), { onClose: closeUpgradeModal });
            }
        }, 500);
    }

    function toggleSaveButton() {
        if ($('li.drop-item').length > 0) {
            $('#submit-button-list').show();
        } else {
            $('#submit-button-list').hide();
        }
    }

    function draggableElement() {
        return $('<div>' + $(this).html() + '</div>');

    }

    function bindDroppableItems() {

        $('.drop-item').droppable({
            over: handleItemOver
        });
    }

    function handleItemOver(event, ui) {
        if (!inSort) {
            append = false;
            dropPosition = $('.drop-item').index($(this));
            $("ul#drop-form li.drop-item:nth-child(" + (getDropPos()) + ")").before(guideElement());
        }
    }

    function bindSettingsActions(obj) {

    }

    function guideElement() {
        // remove previous guides
        removeGuide();

        // return new guides
        return $("<li id='guide-item' class='guido'></li>");
    }

    function getDropPos() {
        return dropPosition + 1;
    }

    function generateItem(controltype, orderId, isAvailable) {
        var availability = (isAvailable) ? "available" : "unavailable";
        var uniqueId = parseInt($('li.drop-item').length + 1);
        var uniqueIdStr = 'drop-item-' + uniqueId;
        var isIdUnique = $('#' + uniqueIdStr).length == 0;

        while (!isIdUnique) {
            uniqueId++;
            uniqueIdStr = 'drop-item-' + uniqueId;
            isIdUnique = $('#' + uniqueIdStr).length == 0;
        }


        activeItemId = uniqueId;
        activeControl = controltype.toLowerCase();

        var templateData = [
            { id: uniqueId, order: orderId, fieldType: activeControl, isavailable: isAvailable }
        ];


        var listItem = $('<li id="' + uniqueIdStr + '" class="drop-item active ' + availability + '" data-control-type="' + activeControl + '" data-dom-id="' + uniqueId + '"></li>')
        var templateName = 'form-field-' + activeControl + '-template';
        var renderedTemplate = $('#' + templateName).tmpl(templateData);
        listItem.append(renderedTemplate);
        return listItem;

    }

    function removeGuide() {
        $('.guido').remove();
    }

    function loadProperties() {
        if (activeItemId == 0) {
            $('#field-property-container').hide();
            $('#form-property-container').show();
        } else {
            $('#field-property-container').show();
            $('#form-property-container').hide();

            // show applicable properties
            $('#field-property-table').find('tr').show();
            $('#field-property-table').find('tr').not('.' + activeControl).hide();

            //setup "subscriber channel" properties
            var settingInputs = $('#field-property-table').find('tr:visible').find('input');
            var settingDropDowns = $('#field-property-table').find('tr:visible').find('select');
            assignSettingValues(settingInputs);
            assignSettingValues(settingDropDowns);

            // console.log(getPropertiesFor(activeItemId));

        }
    }

    function assignSettingValues(settingFields) {
        settingFields.removeAttr('data-sub-channel')
        settingFields.each(function (index, item) {
            var fieldPropertyType = $(item).attr('data-field-property');
            if (fieldPropertyType) {
                var newSubChannel = 'sub-' + fieldPropertyType + '-' + activeItemId;
                var currHiddenValId = fieldPropertyType + '-prop-' + activeItemId;
                var currHiddenElem = $('#' + currHiddenValId);
                $(item).attr('data-sub-channel', newSubChannel);  // change the subscription channel
                $(item).val(currHiddenElem.val());                // pull value from hidden field
            }
        });
    }

    function bindSortable() {
        // bind sortable drop form
        $('#drop-form').sortable({
            handle: '.drag-icon',
            placeholder: 'place-holder',
            start: startSort,
            update: updateSort
        });
    }

    function updateSort(event, ui) {
        inSort = false;
        refreshSortOrder();
    }

    function startSort(event, ui) {
        inSort = true;
    }

    function refreshSortOrder() {
        $('ul#drop-form').find('li.drop-item').each(function (index, item) {
            var domId = $(item).attr('data-dom-id');
            var orderValElement = $('#order-prop-' + domId);
            orderValElement.val(index);

        });
    }

    function bindEditables() {
        // bind editable-labels
        $('.editable').editable(updateEditableField, {
            onblur: 'submit',
            cssclass: 'ignore',
            maxlength: 40
        });

        $('.form-editable').editable(updateFormEditableField, {
            onblur: 'submit',
            cssclass: 'ignore',
            maxlength: 20
        });
    }

    // callback function after a label/or other inline-editable field
    // has been updated
    function updateEditableField(value, settings) {
        var pubId = $(this).closest('li.drop-item').attr('data-dom-id');
        var publisherType = $(this).get(0).tagName.toLowerCase();
        publisherType = publisherType == 'h2' ? 'text' : publisherType;
        if (value.length == 0) {
            value = "Click to edit";
        }
        doFieldSettingUpdates(pubId, publisherType, value);
        $('#isAltered').val(1)
        return (value);
    }


    function updateFormEditableField(value, settings) {
        if (value.length == 0) {
            value = "Registration";
        }
        doFieldSettingUpdates("0", "title", value);
        $('#isAltered').val(1)
        return (value);
    }

    // assigns updated settings to hidden input fields and other targets
    // that need to be updated in realtime
    function doFieldSettingUpdates(publisherId, publisherType, valueToPublish) {
        var subIdentifier = 'sub-' + publisherType + '-' + publisherId;
        var inputSubscribers = $('input[data-sub-channel=' + subIdentifier + ']');
        inputSubscribers.val(valueToPublish);

        var labelSubscribers = $('label[data-sub-channel=' + subIdentifier + ']');
        labelSubscribers.text(valueToPublish);

        var titleSubscribers = $('h2[data-sub-channel=' + subIdentifier + ']');
        titleSubscribers.text(valueToPublish);

        applyRealtimeChange(publisherId, publisherType, valueToPublish)

        //console.log(getPropertiesFor(publisherId));
    }

    function applyRealtimeChange(domId, changeType, value) {
        var targetContainer = $('#drop-item-' + domId);
        var _controlType = targetContainer.attr('data-control-type');
        switch (changeType) {
            case "isrequired":
                if (value == "True") {
                    targetContainer.find(".required").removeClass('hidden').addClass('visible');
                } else {
                    targetContainer.find(".required").removeClass('visible').addClass('hidden');
                }
                break;
            case "maxchars":
                targetContainer.find('input[type=text]').attr("maxlength", value);
                break;
            case "helptext":
                var helpIcon = targetContainer.find('.help-icon');
                helpIcon.attr("title", value);
                if (value.length > 0) {
                    helpIcon.show();
                } else {
                    helpIcon.hide();
                }
                $U.bindTipsies();
                break;
            case "hint":
                var inputField = targetContainer.find('input[type=text]');
                inputField.attr("title", value);
                $U.bindWatermarks();
                break;
            case "options":
                var optionsArray = (value.length > 0) ? value.split(',') : "Option 1, Option 2".split(',');

                if (_controlType == "dropdownlist") {
                    // bind options to select list   
                    var selectList = targetContainer.find('select');
                    selectList.find('option').remove();
                    $.each(optionsArray, function (index, item) {
                        selectList.append('<option>' + item + '</option');
                    });

                } else if (_controlType == "radiobutton") {
                    //bind options to radio button list
                    var optionList = targetContainer.find('.option-list');
                    optionList.find('li').remove();
                    $.each(optionsArray, function (index, item) {
                        optionList.append('<li><input type="radio" value="' + item + '" name="radiogroup-' + domId + '" /><label>' + item + '</label></li>')
                    });
                } else if (_controlType == "checkbox") {
                    // bind options to checkbox list
                    var optionList = targetContainer.find('.option-list');
                    optionList.find('li').remove();
                    $.each(optionsArray, function (index, item) {
                        optionList.append('<li><input type="checkbox" value="' + item + '" /><label>' + item + '</label></li>')
                    });
                }
                break;
            case "selectedoption":
                if (_controlType == "dropdownlist") {
                    var selectList = targetContainer.find('select');
                    selectList.val(value);
                } else if (_controlType == "radiobutton") {
                    targetContainer = $('#drop-item-' + domId + ' :input[value="' + value + '"]');
                    targetContainer.removeAttr('checked');
                    targetContainer.attr('checked', 'checked')
                }

                break;
            case "minimumage":
                var d = new Date();
                var thisYear = d.getFullYear();
                var maxAge = parseInt(targetContainer.find('#maximumage-prop-' + domId).val());
                var minAge = (parseInt(value) >= maxAge) ? maxAge - 1 : parseInt(value);
                var yearddl = targetContainer.find('.birth-year')
                yearddl.find('option').remove();

                var startYear = parseInt(thisYear - maxAge);
                var endYear = parseInt(thisYear - minAge);

                // console.log("start: " + startYear + "\n end:" + endYear + "\nminage:" + minAge + "\nmaxAge:" + maxAge);
                for (i = endYear; i >= startYear; i--) {
                    yearddl.append($('<option>' + i + '</option>'));
                }
                yearddl.prepend($('<option selected="true"></option>'));

                $('#maxAge').val(maxAge);
                $('#minAge').val(minAge);
                break;
            case "maximumage":
                var d = new Date();
                var thisYear = d.getFullYear();
                var minAge = parseInt(targetContainer.find('#minimumage-prop-' + domId).val());
                var maxAge = (parseInt(value) <= minAge) ? minAge + 1 : parseInt(value);
                var yearddl = targetContainer.find('.birth-year')
                yearddl.find('option').remove();

                var startYear = parseInt(thisYear - maxAge);
                var endYear = parseInt(thisYear - minAge);

                // console.log("start: " + startYear + "\n end:" + endYear + "\nminage:" + minAge + "\nmaxAge:" + maxAge);
                for (i = endYear; i >= startYear; i--) {
                    yearddl.append($('<option>' + i + '</option>'));
                }
                yearddl.prepend($('<option selected="true"></option>'));


                $('#maxAge').val(maxAge);
                $('#minAge').val(minAge);
                break;

        }
    }



    function activateField(parentItem) {
        $('li.drop-item').removeClass('active');
        parentItem.addClass('active');
        activeControl = parentItem.attr('data-control-type');
        activeItemId = parentItem.attr('data-dom-id');
        loadProperties();
    }

    // binds all settings fields with a function
    // that triggers a broadcast of changes to subscribers
    function bindPublishers() {
        $('.is-publisher').live('change', function () {
            var propType = $(this).attr('data-field-property')
            if (propType) {
                doFieldSettingUpdates(activeItemId, propType, $(this).val());
            }

            $('#isAltered').val(1);
        });
    }

    function getPropertiesFor(domid) {
        var props = "Type: " + $('#type-prop-' + domid).val() + "\n" +
                    "Text: " + $('#text-prop-' + domid).val() + "\n" +
                    "Label: " + $('#label-prop-' + domid).val() + "\n" +
                    "Max Chars: " + $('#maxchars-prop-' + domid).val() + "\n" +
                    "Is Reqd: " + $('#isrequired-prop-' + domid).val() + "\n" +
                    "Options: " + $('#options-prop-' + domid).val() + "\n" +
                    "Selected: " + $('#selected-prop-' + domid).val() + "\n" +
                    "Hover Txt: " + $('#hovertext-prop-' + domid).val() + "\n" +
                    "Hint: " + $('#hint-prop-' + domid).val() + "\n" +
                    "Min Age: " + $('#minimumage-prop-' + domid).val() + "\n" +
                    "Max Age: " + $('#maximumage-prop-' + domid).val() + "\n" +
                    "Max filesize: " + $('#minfilesize-prop-' + domid).val() + "\n" +
                    "Min filesize: " + $('#maxfilesize-prop-' + domid).val() + "\n" +
                    "valid extensions: " + $('#validextensions-prop-' + domid).val() + "\n" +
                    "Help Txt: " + $('#helptext-prop-' + domid).val() + "\n" +
                    "Order: " + $('#order-prop-' + domid).val() + "\n";

        return props;

    }

    function handleSaveCallback(content) {
        $('#IsAutoSave').val('false');
        var response = $.parseJSON(content.responseText);
        if (response.success) {
            if (!response.isautosave) {
                $U.writeSuccessToContainer($('#message'), response.message);
            }
            bindFieldIds(response.fieldids);
            $('#isAltered').val(0);
            $('#autosave-container').hide();
        } else {
            if (!response.isautosave) {
                $U.writeErrorToContainer($('#message'), response.error)
            }
        }

        enableSubmitButtons();
    }

    function bindFieldIds(idsObj) {
        // console.log(idsObj)
        if (idsObj && idsObj.length > 0) {

            for (var i = 1; i <= idsObj.length; i++) {

                var idObj = idsObj[i - 1];
                var hiddenField = $('#fieldid-prop-' + idObj.domid);
                if (hiddenField.length > 0) {
                    //  console.log('updating value:' + i);
                    hiddenField.val(idObj.id);
                }
            }
        }
    }

    function refreshForm() {
        if ($('ul#drop-form').find('li.drop-item').length > 0) {
            $('.publish-item').show();
            $('#isAltered').val(1);
            if ($('li.prompt-item').length > 0) {
                $('li.prompt-item').remove();
            }
        } else {
            $('.publish-item').hide();
            if ($('li.prompt-item').length == 0) {
                var promptTemplate = $('#form-field-prompt-template').tmpl();
                $('ul#drop-form').append(promptTemplate)
            }
        }
    }

    function bindAutoSave() {
        setInterval(function () {
            if ($('#isAltered').val() == "1") {
                $('#autosave-container').show();
                disableSubmitButtons();
                $('#IsAutoSave').val('true');
                $('#main-form').submit();
            }

        }, 15000);
    }

    function disableSubmitButtons() {
        $('.save-button').attr("disabled", "disabled");
        $('.save-button').addClass("disabled");
    }

    function enableSubmitButtons() {
        $('.save-button').removeAttr("disabled", "disabled");
        $('.save-button').removeClass("disabled");
    }

    function handleBeginSave() {
        // $U.log('beginning save');
        disableSubmitButtons();
        $('#isAltered').val(0); // prevent autosave while save in progress
    }

    function bindNavigateAway() {
        window.onbeforeunload = function () {
            if ($('#isAltered').val() == "1") {
                return "Are you sure you want to leave this page? Your changes have not been saved.";
            }

        };
    }

    function init() {
        bindBasicActions();
        bindDragAndDrop();
        bindSortable();
        bindEditables();
        bindPublishers();
        bindAutoSave();
        toggleSaveButton();
        bindNavigateAway();
    }

    return {
        init: init,
        handleSaveCallback: handleSaveCallback,
        handleBeginSave: handleBeginSave
    }
} ();