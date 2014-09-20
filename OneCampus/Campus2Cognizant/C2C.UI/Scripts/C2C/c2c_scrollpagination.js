; (function ($) {
    $.ScrollPagination = function (element, options) {

        // plugin's default options
        // this is private property and is  accessible only from inside the plug-in
        var defaults = {
            contentPage: null,
            contentData: {},
            pagerFieldName: 'page',
            totalPageCount: 1,
            heightOffset: 0,
            onBeforeLoading: null,
            OnLoad: null,
            onAfterLoading: null,
            OnEnd: null
        }

        // to avoid confusions, use "plugin" to reference the 
        // current instance of the object
        var plugin = this;
        var isProcessing = false;
        var lastLoadedPage = 0;

        // this will hold the merged default, and user-provided options
        // plug-in's properties will be available through this object like:
        // plugin.settings.propertyName from inside the plug-in or
        // element.data('ScrollPagination').settings.propertyName from outside the plug-in, 
        // where "element" is the element the plug-in is attached to;
        plugin.settings = {}

        var $element = $(element), // reference to the jQuery version of DOM element
             element = element;    // reference to the actual DOM element

        // the "constructor" method that gets called when the object is created
        plugin.init = function () {
            // the plug-in's final properties are the merged default and 
            // user-provided options (if any)
            plugin.settings = $.extend({}, defaults, options);

            //Loading default page
            loadContent();

            // code goes here
            $element.scroll(function (event) {
                if (!isProcessing) {
                    isProcessing = true;
                    var mayLoadContent = $element.scrollTop() + $element.innerHeight() >= $element[0].scrollHeight;
                    if (mayLoadContent) {
                        if (lastLoadedPage < plugin.settings.totalPageCount)
                            loadContent();
                        else {
                            event.preventDefault();
                            // Check User has OnEnd event if so trigger the event
                            if (plugin.settings.OnEnd != null) {
                                plugin.settings.OnEnd();
                            }
                        }
                    }
                    else {
                        event.preventDefault();
                    }

                    isProcessing = false;
                }
                else {
                    event.preventDefault();
                }
            });
        }

        // private methods to retrieve content and load 
        var loadContent = function () {
            // Check whether all pages are loaded or not
            if (lastLoadedPage < plugin.settings.totalPageCount) {
                // Check User has onBeforeLoading event if so trigger the event
                if (plugin.settings.onBeforeLoading != null) {
                    plugin.settings.onBeforeLoading();
                }
                // Constructing data argument with pagination page number parameter.
                var data = plugin.settings.contentData;
                data[plugin.settings.pagerFieldName] = lastLoadedPage + 1;
                // Ajax call to get the data
                $.ajax({
                    type: 'GET',
                    url: plugin.settings.contentPage,
                    data: data,
                    dataType: "html",
                    cache: false,
                    async: false,
                    success: function (data) {
                        lastLoadedPage++;
                        // Check User has OnLoad event if so trigger the event
                        if (plugin.settings.OnLoad != null) {
                            plugin.settings.OnLoad(data);
                        }
                        else {
                            $element.append(data);
                        }

                        // To enable scrollbar if the content is less than container
                        var hasVerticalScrollbar = element.scrollHeight > element.clientHeight;
                        if (hasVerticalScrollbar == false && lastLoadedPage < plugin.settings.totalPageCount) {
                            loadContent();
                        }
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        //alert('Error has occurred, please try again after some time.');
                    }
                });

                // Check User has onAfterLoading event if so trigger the event
                if (plugin.settings.onAfterLoading != null) {
                    plugin.settings.onAfterLoading();
                }
            }
            else {
                // Check User has OnEnd event if so trigger the event
                if (plugin.settings.OnEnd != null) {
                    plugin.settings.OnEnd();
                }
            }
        }

        // fire up the plug-in!
        // call the "constructor" method
        plugin.init();

    }

    // add the plug-in to the jQuery.fn object
    $.fn.ScrollPagination = function (options) {

        // iterate through the DOM elements we are attaching the plug-in to
        return this.each(function () {

            // if plug-in has not already been attached to the element
            if (undefined == $(this).data('ScrollPagination')) {

                // create a new instance of the plug-in
                // pass the DOM element and the user-provided options as arguments
                var plugin = new $.ScrollPagination(this, options);

                // in the jQuery version of the element
                // store a reference to the plug-in object
                // you can later access the plug-in and its methods and properties like
                // element.data('ScrollPagination').publicMethod(arg1, arg2, ... argn) or
                // element.data('ScrollPagination').settings.propertyName
                $(this).data('ScrollPagination', plugin);
            }
        });
    }
})(jQuery);