if (typeof jQuery === "undefined") {
	throw new Error("jQuery plugins need to be before this file");
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
		$('.btn-wishlist').on('click', function () {
			if ($(this).hasClass('active')) {
				$(this).removeClass('active');
				var data = {
					icon: 'far fa-times-circle',
					title: '',
					message: 'Este producto ha sido removido de su lista de deseos.',
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
					message: 'Este producto ha sido añadido a su lista de deseos.',
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
                url: '/Account/Provinces',
                data: { id: id },
                success: function (e) {
                    var modelsHtml = "";
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
                url: '/Account/Communes',
                data: { id: id },
                success: function (e) {
                    var modelsHtml = "";
                    console.log(e);
                    $.each(e, function (a, b) {
                        modelsHtml += "<option value='" + b.id + "'>" + b.nombre + "</option>";
                    })
                    $("#strComunaNationale").html(modelsHtml);
                }
            })
        })
    }
}

$(function () {
	//$.AdminJs.input.activate();
	$.AdminJs.totop.activate();
	$.AdminJs.navBar.activate();
	$.AdminJs.mobile.activate();
	$.AdminJs.leftMenu.activate();
	$.AdminJs.compare.activate();
	//setTimeout(function () { $('.page-loader-wrapper').fadeOut(); }, 50);
    $.AdminJs.animateLinks.activate();
    $.AdminJs.slider.activate();
    
    $('[data-toggle="tooltip"]').tooltip();
    $('[data-toggle="popover"]').popover();
});

$.AdminJs.navBar = {
	activate: function () {
		var $nav = $(".navbar-sticky");
		var $img = $(".navbar-brand");
		if ($nav.offset().top > 40) {
			$nav.toggleClass('scrolled');
			$nav.toggleClass('navbar-stuck');
			$img.toggleClass('scrolled');
		}
		$(document).scroll(function () {
			var $nav = $(".navbar-sticky");
			var $img = $(".navbar-brand");
			$nav.toggleClass('scrolled', $(this).scrollTop() > 40);
			$nav.toggleClass('navbar-stuck', $(this).scrollTop() > 40);
			$img.toggleClass('scrolled', $(this).scrollTop() > 40);
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
		}, function () {
			$('.mg-site-search').removeClass('in-view');
			$('.mg-search').removeClass('off-view');
			$('.icon-default').removeClass('icon-default-hover');
			$('.icon-hover').removeClass('icon-hover-hover');
			$('.mg-search-overlay').hide();
		});

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
	}
}

$.AdminJs.leftMenu = {
	activate: function () {
		$('.mg-widget-categories li.has-children a').on('click', function (e) {
			e.preventDefault();
			var p = $(this).parent('li');
			$('.mg-widget-categories li.has-children.expanded').not(p).removeClass('expanded');
			if (p.hasClass('expanded')) {
				p.removeClass('expanded');
			} else {
				p.addClass('expanded');
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
			$('.mg-slide-menu-wrapper.off-view').css('height', x.height());
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
		if (window.matchMedia('(max-width: 767px)').matches) {
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
            max: 1000,
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