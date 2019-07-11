if (typeof jQuery === "undefined") {
	throw new Error("jQuery plugins need to be before this file");
}
$.Dir ={
    en: function (x) {
        var englishDic = {
            "quoteAdd": "This product has been added to your quote list.",
            "quoteRemove": "This product has been remove to your quote list."
        }

        return englishDic[x];
    },
    es: function (x) {
        var spanishDic = {
            "quoteAdd": "Este producto ha sido añadido a su lista de cotización.",
            "quoteRemove": "Este producto ha sido removido de su lista de cotización."
        }

        return spanishDic[x];

    }
}

$.AdminJs = {};

$.AdminJs.Test = {
	activate: function () {
		$('#Test').on('submit', function (e) {
			e.preventDefault();
			var x = $(this).serialize();
			$.ajax({
				type: 'POST',
				url: 'User/LogIn/',
				data: x,
				cache: false,
				success: function (e) {
					console.log(e);
					if (e === "yes") {
						window.location.href = "/";
					}
				}
			});
		});
	}
}

$.AdminJs.LogUp = {
	activate: function () {
		$('#LogUp').on('submit', function (e) {
			e.preventDefault();
			var x = $(this).serialize();
			$.ajax({
				type: 'POST',
				url: 'User/LogUp/',
				data: x,
				cache: false,
				success: function (e) {
					window.location.href = "User/Iniciar/";
				}
			});
		});
	}
}

$.AdminJs.compare = {
	activate: function () {
        $('.mg-product-wish').on('click', function () {
			if ($(this).hasClass('active')) {
				$(this).removeClass('active');
				var data = {
					icon: 'far fa-times-circle',
                    title: '',
                    message: TranslateText("quoteRemove"),
					type: 'danger',
					from: 'top',
					align: 'right',
					mouse: 'pause',
					enter: 'animated fadeInRight',
					exit: 'animated fadeOutRight'
				}
				myAlert(data)
			} else {
				$(this).addClass('active');
				var data = {
					icon: 'far fa-check-circle',
					title: '',
                    message: TranslateText("quoteAdd"),
					type: 'success',
					from: 'top',
					align: 'right',
					mouse: 'pause',
					enter: 'animated fadeInRight',
					exit: 'animated fadeOutRight'
				}
				myAlert(data)
			}
		})
		$('.btn-compare').on('click', function () {
			if ($(this).hasClass('active')) {
				$(this).removeClass('active');
				var data = {
					icon: 'far fa-times-circle',
					title: '',
					message: 'Este producto ha sido removido de su lista de comparación.',
					type: 'danger',
					from: 'top',
					align: 'right',
					mouse: 'pause',
					enter: 'animated fadeInRight',
					exit: 'animated fadeOutRight'
				}
				myAlert(data)
			} else {
				$(this).addClass('active');
				var data = {
					icon: 'far fa-check-circle',
					title: '',
					message: 'Este producto ha sido añadido a su lista de comparación.',
					type: 'success',
					from: 'top',
					align: 'right',
					mouse: 'pause',
					enter: 'animated fadeInRight',
					exit: 'animated fadeOutRight'
				}
				myAlert(data)
			}
		})
	}
}

$.AdminJs.addAddress = {
    activate: function () {
        $('input[name="strType"]').on('change', function () {
            var id = $(this).attr('id');
            if (id === "strInternational") {
                if ($('#addNationalAddressSecction').is(':Visible')) {
                    $('#addNationalAddressSecction').slideUp('fast');
                }
                $('#addInternationalAddressSecction').slideDown('slow');
            } else if (id === "strNational") {
                if ($('#addInternationalAddressSecction').is(':Visible')) {
                    $('#addInternationalAddressSecction').slideUp('fast');
                }
                $('#addNationalAddressSecction').slideDown('slow');
            }
        })

        $('#strStatesNationale').on('change', function () {
            var id = $(this).val();
            $.ajax({
                type: 'GET',
                dataType: "JSON",
                url: '/en/Account/Provinces/',
                data: { id: id },
                success: function (e) {
                    var modelsHtml = "<option value=''></option>";
                    $.each(e, function (a, b) {
                        modelsHtml += "<option value='" + b.id + "'>" + b.nombre + "</option>";
                    })
                    $("#strProvinciaNationale").html(modelsHtml);
                }
            })
        })

        $('#strProvinciaNationale').on('change', function () {
            var id = $(this).val();
            $.ajax({
                type: 'GET',
                dataType: "JSON",
                url: '/en/Account/Communes/',
                data: { id: id },
                success: function (e) {
                    var modelsHtml = "<option value=''></option>";
                    $.each(e, function (a, b) {
                        modelsHtml += "<option value='" + b.id + "'>" + b.nombre + "</option>";
                    })
                    $("#strComunaNationale").html(modelsHtml);
                }
            })
        })
    }
}

$.AdminJs.cart = {
    active: function () {
        $('.remove-from-cart').on('click', function () {
            $(this).parents('tr')
                .children('td, th')
                .animate({
                    padding: 0
                })
                .wrapInner('<div />')
                .children()
                .slideUp(function () {
                    $(this).closest('tr').remove();
                });
        })
    }
}

$.AdminJs.checkOut = {
    active: function () {
        var _this = this;

        _this.checkoutSteps();
        _this.validateForm();
        $('#strStatesNationale').on('change', function () {
            var id = $(this).val();
            $.ajax({
                type: 'GET',
                dataType: "JSON",
                url: '/en/Account/Provinces/',
                data: { id: id },
                success: function (e) {
                    var modelsHtml = "<option value=''></option>";
                    $.each(e, function (a, b) {
                        modelsHtml += "<option value='" + b.id + "'>" + b.nombre + "</option>";
                    })
                    $("#strProvinciaNationale").html(modelsHtml);
                }
            })
        })

        $('#strProvinciaNationale').on('change', function () {
            var id = $(this).val();
            $.ajax({
                type: 'GET',
                dataType: "JSON",
                url: '/en/Account/Communes/',
                data: { id: id },
                success: function (e) {
                    var modelsHtml = "<option value=''></option>";
                    $.each(e, function (a, b) {
                        modelsHtml += "<option value='" + b.id + "'>" + b.nombre + "</option>";
                    })
                    $("#strComunaNationale").html(modelsHtml);
                }
            })
        })

        $('#strStatesNationaleAnother').on('change', function () {
            var id = $(this).val();
            $.ajax({
                type: 'GET',
                dataType: "JSON",
                url: '/en/Account/Provinces/',
                data: { id: id },
                success: function (e) {
                    var modelsHtml = "<option value=''></option>";
                    $.each(e, function (a, b) {
                        modelsHtml += "<option value='" + b.id + "'>" + b.nombre + "</option>";
                    })
                    $("#strProvinciaNationaleAnother").html(modelsHtml);
                }
            })
        })

        $('#strProvinciaNationaleAnother').on('change', function () {
            var id = $(this).val();
            $.ajax({
                type: 'GET',
                dataType: "JSON",
                url: '/en/Account/Communes/',
                data: { id: id },
                success: function (e) {
                    var modelsHtml = "<option value=''></option>";
                    $.each(e, function (a, b) {
                        modelsHtml += "<option value='" + b.id + "'>" + b.nombre + "</option>";
                    })
                    $("#strComunaNationaleAnother").html(modelsHtml);
                }
            })
        })

        $('input[name="sameAddress"]').on('change', function () {
            if ($(this).val() == "1") {
                if ($('#anotherAddress').is(':visible')) {
                    $('#anotherAddress').addClass('d-none');
                    var height = $(this).parents('fieldset').outerHeight();
                    $(this).parents('.content.clearfix').css({ "height": height });
                }
            } else {
                if ($('#anotherAddress').is(':visible')) {
                } else {
                    $('#anotherAddress').removeClass('d-none');
                    var height = $(this).parents('fieldset').outerHeight();
                    $(this).parents('.content.clearfix').css({ "height": height });
                }
            }
        })

        $('input[name="samePerson"]').on('change', function () {
            if ($(this).val() == "1") {
                if ($('#anotherPerson').is(':visible')) {
                    $('#anotherPerson').addClass('d-none');
                    var height = $(this).parents('fieldset').outerHeight();
                    $(this).parents('.content.clearfix').css({ "height": height });
                }
            } else {
                if ($('#anotherPerson').is(':visible')) {
                } else {
                    $('#anotherPerson').removeClass('d-none');
                    var height = $(this).parents('fieldset').outerHeight();
                    $(this).parents('.content.clearfix').css({ "height": height });
                }
            }
        })
    },
    checkoutSteps: function () {
        var form = $("#CheckOutSteps");
        var url = window.location.pathname;
        url = url.split('/');
        var lang = url[1];
        var label;

        if (lang === "es") {
            label = {
                next: "Continuar",
                previous: "Atrás",
                finish: "Terminar"
            };
        } else {
            label = {
                next: "Continue",
                previous: "Back",
                finish: "Finish"
            };
        }

        form.steps({
            headerTag: "h3",
            bodyTag: "fieldset",
            transitionEffect: "slideLeft",
            autoFocus: true,
            labels: label,
            onStepChanging: function (event, currentIndex, newIndex) {
                form.validate().settings.ignore = ":disabled,:hidden";
                if (currentIndex < newIndex) {
                    // To remove error styles
                    form.find(".body:eq(" + newIndex + ") label.error").remove();
                    form.find(".body:eq(" + newIndex + ") .error").removeClass("error");
                }

                if (newIndex == 1) {
                    $('input[name="strShippingType"]').on('change', function () {
                        var shippingType = $(this).val();
                        if (shippingType == 1) {
                            $('#ShippingTypeAddress').css({ "display": "none" });
                            $('#ShippingTypeStore').show();
                            var height = $(this).parents('fieldset').outerHeight();
                            $(this).parents('.content.clearfix').css({ "height": height });
                        } else if (shippingType == 2) {
                            $('#ShippingTypeStore').css({ "display": "none" });
                            $('#ShippingTypeAddress').show();
                            if ($('input[name="sameAddress"]').is(':checked') && $('input[name="samePerson"]').is(':checked')) {
                                form.validate().settings.ignore = ":disabled,:hidden,#anotherPerson :input, #anotherAddress :input";
                            }
                            var height = $(this).parents('fieldset').outerHeight();
                            $(this).parents('.content.clearfix').css({ "height": height });
                        }
                    })
                }

                if (newIndex == 2) {
                    if ($('input[name="strShippingType"]:checked').val() == 2) {
                        $('input[id="cash"]').parent().addClass('d-none');
                    } else {
                        $('input[id="cash"]').parent().removeClass('d-none');
                    }
                }
                return true;
            },
            onStepChanged: function (event, currentIndex, newIndex) {

            },
            /*onFinishing: function (event, currentIndex)
            {
                form.validate().settings.ignore = ":disabled";
                return form.valid();
            }*/
            onFinished: function (event, currentIndex) {
                var x = $('input[name="credit-card"]:checked');
                if (x.val() === "webpay") {
                    alert('to webPay')
                } else if (x.val() === "transferencia") {
                    alert('to Transference')
                } else {
                    alert('must select')
                }
            }
        });
    },
    validateForm: function () {
        var form = $("#CheckOutSteps");
        var lang = $('body').data('lang');
        if (lang == "es") {
            jQuery.extend(jQuery.validator.messages, {
                required: "Campo requerido.",
            });
        }
    }
}

$(function () {
	//$.AdminJs.input.activate();
	$.AdminJs.totop.activate();
	$.AdminJs.navBar.activate();
	$.AdminJs.mobile.activate();
	$.AdminJs.leftMenu.activate();
	$.AdminJs.compare.activate();
    $.AdminJs.animateLinks.activate();
    
    $('[data-toggle="tooltip"]').tooltip();
    $('[data-toggle="popover"]').popover();

});

document.onreadystatechange = function () {
    var state = document.readyState
    if (state == 'interactive') {
        $('body').css({
            overflow: 'hidden'
        });
    }
    else if (state == 'complete') {
        document.getElementById('interactive');
        document.getElementById('page-loader-wrapper').style.display = "none";
        $('body').css({
            overflow: 'auto'
        });
    }
}

$.AdminJs.navBar = {
    activate: function () {
        var _this = this;
        var $nav = $(".navbar-sticky.mg-big-screen");
        var prevScrollpos = window.pageYOffset;
		if ($nav.offset().top > 40) {
			$nav.toggleClass('scrolled');
			$nav.toggleClass('navbar-stuck');
        }

		$(document).scroll(function () {
            var $nav = $(".navbar-sticky.mg-big-screen");
			$nav.toggleClass('scrolled', $(this).scrollTop() > 40);
            $nav.toggleClass('navbar-stuck', $(this).scrollTop() > 40);

            

            var currentScrollPos = window.pageYOffset;
            if (prevScrollpos > currentScrollPos) {
                document.getElementById("navbar").style.bottom = "0";
            } else {
                document.getElementById("navbar").style.bottom = "-100px";
            }
            prevScrollpos = currentScrollPos;
        });

		jQuery.fn.clickToggle = function (a, b) {
			function cb() { [b, a][this._tog ^= 1].call(this); }
			return this.on("click", cb);
        };

		$('.mg-fix-bugg').clickToggle(function () {
			$('.mg-search-overlay').show();
			$('.mg-search').addClass('off-view');
			$('.mg-site-search').addClass('in-view');
			$('.icon-default').addClass('icon-default-hover');
			$('.icon-hover').addClass('icon-hover-hover');
        },
            function () {
			$('.mg-site-search').removeClass('in-view');
			$('.mg-search').removeClass('off-view');
			$('.icon-default').removeClass('icon-default-hover');
			$('.icon-hover').removeClass('icon-hover-hover');
			$('.mg-search-overlay').hide();
            });

        $('#mg-profile-toggle').on('click', function () {
            $('#profileMenuMobile').toggleClass('open');
            $('body').toggleClass('modal-open');
        })

        $('.mg-clear-search').on('click', function () {
            $('input[name="search"]').val('');
        })
    },
    hideNav: function () {
        $("[data-nav-status='toggle']").removeClass("is-visible").addClass("is-hidden");
    },
    showNav: function () {
        $("[data-nav-status='toggle']").removeClass("is-hidden").addClass("is-visible");
    }
}

$.AdminJs.animateLinks = {
	activate: function () {
		var _this = this;
		$('#navbarResponsive a').click(function (e) {
			if (this.hash !== "") {
				e.preventDefault();
				var hash = this.hash;
				$('html, body').animate({
					scrollTop: $(hash).offset().top
				}, 800, function () {
					window.location.hash = hash;
				});
			}
		});
		$('#mg-vertical-nav a').click(function (e) {
			if (this.hash !== "") {
				e.preventDefault();
				var hash = this.hash;
				$('html, body').animate({
					scrollTop: $(hash).offset().top
				}, 800, function () {
					window.location.hash = hash;
				});
			}
        });
        $('.scroll-to').click(function (e) {
            if (this.hash !== "") {
                e.preventDefault();
                var hash = this.hash;
                $('html, body').animate({
                    scrollTop: $(hash).offset().top - 50
                }, 800, function () {
                    window.location.hash = hash;
                });
            }
        });
	}
}

$.AdminJs.leftMenu = {
	activate: function () {
		$('.mg-widget-categories li.has-children>a').on('click', function (e) {
			e.preventDefault();
			var p = $(this).parent('li');
			$('.mg-widget-categories li.has-children.expanded').not(p).removeClass('expanded');
			if (p.hasClass('expanded')) {
				p.removeClass('expanded');
			} else {
				p.addClass('expanded');
			}
        })

        $('.has-children-list a').each(function () {
            if ($(this).hasClass('active')) {
                $(this).parents('.has-children').addClass('expanded');
            }
        })
	}
}

$.AdminJs.profileMenu = {
    activate: function () {
        $('.mg-profile-toggle').on('click', function () {
            $('.mg-profile').toggleClass('open');
        })

        $('.avatar').on('click', function () {
            if(!$(this).hasClass('selected'))
            {
                $(this).toggleClass('active');
            }
            if ($('.avatar').not($(this)).hasClass('active'))
            {
                $('.avatar').not($(this)).removeClass('active')
            }
        })
    }
}

$.AdminJs.mobile = {
	activate: function () {
		$('.sub-menu-toggle').on('click', function (e) {
			e.preventDefault();
			$(this).parents('.mg-slide-menu-wrapper').addClass('off-view');
			var x = $(this).parents('li').find('.slideable-submenu');
			//$('.mg-slide-menu-wrapper.off-view').css('height', x.height());
			x.addClass('in-view');
		});
		$('.back-btn').on('click', function (e) {
			e.preventDefault();
			$(this).parent('ul').removeClass('in-view');
			$(this).parents('.mg-slide-menu-wrapper').css('height', $('.mg-slide-menu-wrapper').data('initial-height'));
			$(this).parents('.mg-slide-menu-wrapper').removeClass('off-view');
		});
		$('.mg-sidebar-toggle').on('click', function () {
			$(this).addClass('sidebar-open');
			$('.mg-sidebar-offcanvas').addClass('open');
		})
		$('.mg-sidebar-close').on('click', function () {
			$('.mg-sidebar-offcanvas').removeClass('open');
			$('.mg-sidebar-toggle').removeClass('sidebar-open');
        })
        $('.mobile-menu-toggle').on('click', function (e) {
            e.preventDefault();
            $('#navbarResponsiveMobile').toggleClass('shown');
            $('body').toggleClass('modal-open');
        })
        $('#on-mobile-language-dropdown').on('click', function () {
            $(this).toggleClass('bg-secondary');
            $('#on-mobile-language').toggleClass('open');
        })
	}
}

$.AdminJs.totop = {
	activate: function () {
		var _this = this;
		if (window.matchMedia('(max-width: 991px)').matches) {
			//...
		} else {
			$(window).scroll(function () {
				if ($(this).scrollTop() > 30) {
					$('.totop').fadeIn();
				} else {
					$('.totop').fadeOut();
				}
			});
			$('#backtoTop').click(function () {
				$('body,html').animate({
					scrollTop: 0
				}, 600);
				return false;
			});
		}
	}
}

$.AdminJs.timing = {
    activate: function () {
        var name = function () {
            $.ajax({
                type: 'GET',
                url: 'time/',
                success: function (e) {
                    e = $.parseJSON(e);
                    if (e.h !== "ready") {
                        $('.hours').html(e.h);
                        $('.minutes').html(e.m);
                        $('.seconds').html(e.s);
                    } else {
                        clearInterval(interval);
                    }
                }
            })
        }
        interval = setInterval(name, 1000);
    }
}

$.AdminJs.slider = {
    activate: function(){
        $(".ui-range-slider").slider({
            range: true,
            min: 0,
            max: $('.mg-price-range-slider').data('start-max'),
            values: [$('.mg-price-range-slider').data('start-min'), $('.mg-price-range-slider').data('start-max')],
            slide: function (event, ui) {
                $(".ui-range-value-min").text("$" + ui.values[0] + " - ");
                $(".ui-range-value-max").text("$" + ui.values[1]);
            }
        });
            /*$( ".ui-range-value-min span" ).val( "$" + $( "#slider-range" ).slider( "values", 0 ) +
            " - $" + $( "#slider-range" ).slider( "values", 1 ) );
            $( ".ui-range-value-max span" ).val( "$" + $( "#slider-range" ).slider( "values", 0 ) +
            " - $" + $( "#slider-range" ).slider( "values", 1 ) );*/
    }
}

$.AdminJs.productstooltips = {
    activate: function () {
        $.AdminJs.slider.activate();

        $('#typeView input[type=radio]').on('change', function () {
            $("#typeView").submit();
        });

        $('#sort-by-button-group').on('change', function () {
            var sort = $(this).val();
            var page = getUrlParameter('Page');
            var view = getUrlParameter('View');
            var perPage = getUrlParameter('PerPage');
            var pathname = window.location.pathname;
            if (page != null && perPage != null && view != null) {
                window.location.href = pathname + '?Page=1&PerPage=' + perPage + '&SortedBy=' + sort + '&View=' + view;
            }
            else if (page != null && perPage != null && view == null) {
                window.location.href = pathname + '?Page=1&PerPage=' + perPage + '&SortedBy=' + sort;
            }
            else if (page != null && perPage == null && view == null) {
                window.location.href = pathname + '?Page=1&SortedBy=' + sort;
            }
            else if (page == null && perPage == null && view != null) {
                window.location.href = pathname + '?SortedBy=' + sort + '&View=' + view;
            }
            else if (page == null && perPage != null && view == null) {
                window.location.href = pathname + '?PerPage=' + perPage + '&SortedBy=' + sort;
            }
            else if (page == null && perPage != null && view != null) {
                window.location.href = pathname + '?PerPage=' + perPage + '&SortedBy=' + sort + '&View=' + view;
            }
            else if (page != null && perPage == null && view != null) {
                window.location.href = pathname + '?Page=1&SortedBy=' + sort + '&View=' + view;
            }
            else {
                window.location.href = pathname + '?SortedBy=' + sort;
            }
        });

        $('#perPage').on('change', function () {
            var page = getUrlParameter('Page');
            var view = getUrlParameter('View');
            var sort = getUrlParameter('SortedBy');
            var pathname = window.location.pathname;
            var perPage = $(this).val();
            if (page != null && view != null && sort !== null) {
                window.location.href = pathname + '?Page=1&PerPage=' + perPage + '&SortedBy=' + sort + '&View=' + view;
            }
            else if (page != null && sort != null && view == null) {
                window.location.href = pathname + '?Page=1&PerPage=' + perPage + '&SortedBy=' + sort;
            }
            else if (page != null && sort == null && view == null) {
                window.location.href = pathname + '?Page=1&PerPage=' + perPage;
            }
            else if (page != null && sort == null && view != null) {
                window.location.href = pathname + '?Page=1&PerPage=' + perPage + '&View=' + View;
            }
            else if (page == null && sort != null && view != null) {
                window.location.href = pathname + '?PerPage=' + perPage + '&SortedBy=' + sort + '&View=' + View;
            }
            else if (page == null && sort == null && view != null) {
                window.location.href = pathname + '?PerPage=' + perPage + '&View=' + View;
            }
            else if (page == null && sort != null && view == null) {
                window.location.href = pathname + '?PerPage=' + perPage + '&SortedBy=' + sort;
            }
            else {
                window.location.href = pathname + '?PerPage=' + perPage;
            }
        });
    }
}

$.AdminJs.gallery = {
    activate: function () {
        var container = [];

        $('.gallery-wrapper').find('.gallery-item').each(function () {
            var $link = $(this).find('a'),
                item = {
                    src: $link.attr('href'),
                    w: $link.data('width'),
                    h: $link.data('height'),
                    title: $(this).find('.caption').text()
                };
            container.push(item);
        });

        $('.gallery-wrapper a').on('click', function (e) {
            e.preventDefault();
            var $pswp = $('.pswp')[0],
                options = {
                    index: $('.gallery-wrapper a').index(this),
                    bgOpacity: 0.85,
                    showHideOpacity: true
                };

            var gallery = new PhotoSwipe($pswp, PhotoSwipeUI_Default, container, options);
            gallery.init();
        })
    }
}

$.AdminJs.Countdown = {
    activate: function () {
        var time = $('.countdown').data('time');
        if (time != null) {
            new FlipClock({
                isCountdown: true,
                startTime: time,
                containerElement: $('.countdown'),
                face: {
                    days: {
                        maxValue: 31
                    },
                    hours: {
                        maxValue: 23
                    },
                    minutes: {
                        maxValue: 59
                    },
                    seconds: {
                        maxValue: 59
                    }
                }
            });
        }
    }
}

function myAlert(x) {
	$.notify({
		icon: x['icon'],
		title: x['title'],
		message: x['message']
	},
		{
            type: x['type'],
            showProgressbar: false,
			placement: {
				from: x['from'],
				align: x['align']
			},
			offset: 20,
			spacing: 10,
			mouse_over: x['mouse'],
			animate: {
				enter: x['enter'],
				exit: x['exit']
			}
		});
}

function TranslateText(x) {
    var $pathname = window.location.pathname,
        lang = $pathname.split('/')[1];
    if (lang == "es") {
        return $.Dir.es(x);
    } else if (lang == "en") {
        return $.Dir.en(x);
    }
}

function getUrlParameter(sParam) {
    var sPageURL = window.location.search.substring(1),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
        }
    }
};