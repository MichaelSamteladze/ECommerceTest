var Utilities = {
    MonthShortNames: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    Cookies: {
        Set: function (cname, cvalue, exdays) {

            if (exdays == undefined) { exdays = 7; }

            var d = new Date();
            d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
            var expires = 'expires=' + d.toUTCString();
            document.cookie = cname + '=' + cvalue + ';' + expires + ';path=/';
        },

        Get: function (cname) {
            var name = cname + '=';
            var decodedCookie = decodeURIComponent(document.cookie);
            var ca = decodedCookie.split(';');
            for (var i = 0; i < ca.length; i++) {
                var c = ca[i];
                while (c.charAt(0) == ' ') {
                    c = c.substring(1);
                }
                if (c.indexOf(name) == 0) {
                    return c.substring(name.length, c.length);
                }
            }
            return '';
        },
    },
    Date: {
        ToShortDate: function (date) {
            date = Date.parse(date);
            if (date) {
                date = new Date(date);
                return MonthShortNames[date.getMonth()] + ' ' + date.getDate() + ', ' + date.getFullYear();
            }
            else {
                return null;
            }
        },
        ToDateObject: function (date) {
            date = Date.parse(date);
            if (date) {
                return new Date(date);
            }
            else {
                return null;
            }
        },
        ToMonthDate: function (date) {
            date = Date.parse(date);
            if (date) {
                date = new Date(date);
                return MonthShortNames[date.getMonth()] + ' ' + date.getDate()
            }
            else {
                return null;
            }
        },
        GetDateWithoutTime: function (date) {
            var d1 = Date.parse(date);
            if (d1) {
                var d2 = new Date(d1);
                var Year = d2.getFullYear();
                var Month = (d2.getMonth() + 1);
                var Day = d2.getDate();

                Month = Month < 10 ? ('0' + Month) : Month;
                Day = Day < 10 ? ('0' + Day) : Day;
                return new Date(Year + '-' + Month + '-' + Day + 'T00:00:00');
            }
            else {
                return null;
            }
        },
        DifferenceInDays(date1, date2) {
            date1 = new Date(date1);
            date2 = new Date(date2);

            var timeDiff = Math.abs(date2.getTime() - date1.getTime());
            return Math.ceil(timeDiff / (1000 * 3600 * 24));
        }
    },
    GUP: function (name, url) {
        name = name.replace(/[\[]/, '\\\[').replace(/[\]]/, '\\\]');
        var regexS = '[\\?&]' + name + '=([^&#]*)';
        var regex = new RegExp(regexS);
        if (url == undefined || url == null) {
            url = window.location.href;
        }
        var results = regex.exec(url);
        if (results == null)
            return null;
        else
            return results[1];
    },
    NewID: function () {

        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
        };

        return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
    }    
};

$.fn.extend({
    Show: function () {
        this.removeClass('hidden');
    },
    Hide: function () {
        this.addClass('hidden');
    },
    Toggle: function () {
        if (this.hasClass('hidden')) {
            this.removeClass('hidden');
        }
        else {
            this.addClass('hidden');
        }
    },

    Disable: function () {
        this.addClass('disabled');
        this.attr('disabled', 'disabled');
    },
    Enable: function () {
        this.removeClass('disabled');
        this.removeAttr('disabled');
    },
    ScrollTo: function (selector,milliseconds)
    {
        if (this != null && this != undefined) {
            selector = selector == undefined ? 'html, body' : selector;
            if ($(selector).length > 0) {
                $(selector).animate({
                    scrollTop: this.offset().top - 100
                }, (milliseconds == undefined ? 500 : milliseconds));
            }
        }
    },
    Shake: function (AnimateSide) {
        var _this = $(this)
        _this.addClass('custom-shake');
        setTimeout(function () {
            _this.removeClass('custom-shake');
        }, 300);
    },
    ToSlug: function () {
        var str = this.val();
        str = str.replace(/^\s+|\s+$/g, ''); // trim
        str = str.toLowerCase();

        // remove accents, swap ñ for n, etc
        var from = 'àáäâèéëêìíïîòóöôùúüûñç·/_,:;';
        var to = 'aaaaeeeeiiiioooouuuunc------';
        for (var i = 0, l = from.length ; i < l ; i++) {
            str = str.replace(new RegExp(from.charAt(i), 'g'), to.charAt(i));
        }

        str = str.replace(/[^a-z0-9 -]/g, '') // remove invalid chars
          .replace(/\s+/g, '-') // collapse whitespace and replace by -
          .replace(/-+/g, '-'); // collapse dashes

        return str;
    },
});