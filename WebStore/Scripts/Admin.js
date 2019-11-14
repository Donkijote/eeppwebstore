if (typeof jQuery === "undefined") {
	throw new Error("jQuery plugins need to be before this file");
}
var googleUser;

$.Dir ={
    en: function (x) {
        var englishDic = {
            "quoteAdd": "This product has been added to your quote list.<br/> Click here to go to list.",
            "quoteRemove": "This product has been remove to your quote list.",
            "cartAdd": "This product has been added to your cart list.<br/> Click here to go to list.",
        }

        return englishDic[x];
    },
    es: function (x) {
        var spanishDic = {
            "quoteAdd": "Este producto ha sido añadido a su lista de cotización.<br/> Clic aquí para ver lista.",
            "quoteRemove": "Este producto ha sido removido de su lista de cotización.",
            "cartAdd": "Este producto ha sido añadido a su carrito.<br/> Clic aquí para ver lista."
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

$.AdminJs.LogInUp = {
    activate: function () {
        var _this = this;
        $.AdminJs.reveal.activate();
        $('#LogIn').on('submit', function (e) {
            e.preventDefault();
            if ($(this).valid()) {
                $.AdminJs.Ajax.init({
                    type: 'POST',
                    url: '/en/User/LogIn',
                    data: $(this).serialize(),
                    action: function (e) {
                        if (e.status == "OK") {
                            window.location.reload()
                        }
                    }
                });
            }
        });
        $('#LogInPage').on('submit', function (e) {
            e.preventDefault();
            var url = $(this).data("url");
            if ($(this).valid()) {
                $.AdminJs.Ajax.init({
                    type: 'POST',
                    url: '/en/User/LogIn',
                    data: $(this).serialize(),
                    action: function (e) {
                        if (e.status == "OK") {
                            if (url != "" || url != undefined) {
                                window.location.href = url;
                            } else {
                                window.location.href = $('#url_return').attr('href');
                            }
                            
                        }
                    }
                });
            }
        });
        
        $('.FacebookLogIn').on('click', function (e) {
            e.preventDefault();
            FB.login(function (response) {
                if (response.status === 'connected') {
                    _this.sendFacebookLogIn();
                } else {
                    console.log('Please log into this webpage.');
                }
            }, { scope: 'email, public_profile', return_scopes: true });
        });

        _this.GoogleAppiSetUp("beforeLogin");
        _this.FacebookAppiSetUp("beforeLogin");
    },
    recoveryPassword: function () {
        var _this = this;
        _this.passwordSteps();
        $.AdminJs.reveal.activate();
        $('#RecoveryPassword').on('submit', function (e) {
            e.preventDefault();
        })
    },
    passwordSteps: function () {
        var _this = this;
        var form = $('#RecoveryPassword');
        var url = window.location.pathname;
        url = url.split('/');
        var lang = url[1];
        var label;

        if (lang === "es") {
            label = {
                next: "Enviar Código",
                previous: "Atrás",
                finish: "Cambiar Clave"
            };
        } else {
            label = {
                next: "Send Code",
                previous: "Back",
                finish: "Change password"
            };
        }
        form.steps({
            headerTag: "h3",
            bodyTag: "fieldset",
            transitionEffect: "slideLeft",
            autoFocus: true,
            labels: label,
            onInit: function (event, current) {
                $('ul[role="tablist"]').hide();
                $('.actions > ul > li:first-child').attr('style', 'display:none');
                $('.actions > ul > li:last-child').addClass('hide');
                $('a[href="#next"]').html('OBTENER CÓDIGO');
            },
            onStepChanging: function (event, currentIndex, newIndex) {
                form.validate().settings.ignore = ":disabled,:hidden";
                $.AdminJs.Loading.start();
                if (currentIndex < newIndex) {
                    // To remove error styles
                    form.find(".body:eq(" + newIndex + ") label.error").remove();
                    form.find(".body:eq(" + newIndex + ") .error").removeClass("error");
                }
                
                if (currentIndex == 0) {
                    var email = _this.sendVerificationCode($('#EmailRecovery').val());
                    if (email.status == "OK") {
                        $.AdminJs.Loading.stop();
                        return form.valid();
                    }
                    else {
                        $.AdminJs.Loading.stop();
                        return false;
                    }
                }

                if (currentIndex == 1) {
                    var code = _this.sendValidateVerificationCode($('#CodRecovery').val());
                    if (code.status == "OK") {
                        $.AdminJs.Loading.stop();
                        return form.valid();
                    }
                    else {
                        $.AdminJs.Loading.stop();
                        $.AdminJs.Alert.warning(code.title, code.responseText)
                        return false;
                    }
                }
            },
            onStepChanged: function (event, currentIndex, newIndex) {
                if (currentIndex == 1) {
                    $('a[href="#next"]').html('VALIDAR CÓDIGO');
                }
            },
            onFinishing: function (event, currentIndex, newIndex) {
                if (currentIndex == 2) {
                    var code = _this.sendChangePassword(form.serialize());
                    console.log(code);
                    if (code.status == "OK") {
                        $.AdminJs.Loading.stop();
                        $.AdminJs.Alert.success(code.title, code.responseText, window.location);
                        return form.valid();
                    }
                    else {
                        $.AdminJs.Loading.stop();
                        $.AdminJs.Alert.warning(code.title, code.responseText);
                        return false;
                    }
                }
            },
            onFinished: function (event, currentIndex) {
            }
        })
    },
    validateForm: function () {
        var form = $("#RecoveryPassword");
        var lang = $('body').data('lang');
        if (lang == "es") {
            jQuery.extend(jQuery.validator.messages, {
                required: "Campo requerido.",
            });
        }
        $.validator.addMethod("pwcheck", function (value) {
            return /^[A-Za-z0-9\d=!\-@._*]*$/.test(value) // consists of only these
                && /[a-z]/.test(value) // has a lowercase letter
                && /\d/.test(value) // has a digit
        });
        /*form.validate({
            rules: {
                EmailRecovery: {
                    required: true,
                    email: true
                },
                CodRecovery: {
                    required: true,
                    number: true,
                    digits: true
                },
                PassRecovery: {
                    required: true,
                    minlength: 8
                },
                RePassRecovery: {
                    equalTo: "#PassRecovery",
                    minlength: 8
                }
            },
            messages: {
                CodRecovery: {
                    number: "Solo números son aceptados.",
                    digits: "Solo números son aceptados."
                },
                PassRecovery: {
                    minlength: "Debe tener al menos una longitud de 8 caracteres.",
                    pwcheck: "Debe tener al menos un número, letras, caracter especial(opcional)."
                },
                RePassRecovery: {
                    minlength: "Debe tener al menos una longitud de 8 caracteres.",
                    pwcheck: "Debe tener al menos un número, letras, caracter especial(opcional).",
                    equalTo: "Contraseña deben ser idénticas."
                }
            }
        })*/
    },
    sendVerificationCode: function (x) {
        var resp = "";
        $.ajax({
            type: 'POST',
            url: '/en/User/RetrievePasswordEmail/',
            data: { EmailRecovery: x },
            cache: false,
            async: false,
            success: function (e) {
                resp = e;
            },
            error: function (e) {
                resp = e;
            }
        });
        return resp;
    },
    sendValidateVerificationCode: function (x) {
        var resp = "";
        $.ajax({
            type: 'POST',
            url: '/en/User/RetrievePasswordCode/',
            data: { ValidationCode: x },
            cache: false,
            async: false,
            success: function (e) {
                resp = e;
            },
            error: function (e) {
                resp = e;
            }
        });
        return resp;
    },
    sendChangePassword: function (x) {
        var resp = "";
        $.ajax({
            type: 'POST',
            url: '/en/User/ChangePassword/',
            data: x,
            cache: false,
            async: false,
            success: function (e) {
                resp = e;
            },
            error: function (e) {
                resp = e;
            }
        });
        return resp;
    },
    sendFacebookLogIn: () =>{
        FB.api('/me', { fields: 'id,name,email' }, function (response) {
            console.log(response)
            $.AdminJs.Ajax.init({
                type: 'POST',
                url: '/en/User/FacebookLogIn/',
                data: { Id: response.id, Name: response.name, Email: response.email, Photo: response.picture },
                action: (x) => {
                    if (x.status == "info") {
                        FB.logout(function (response) {
                            if (response.status == "unknown") {
                                $.AdminJs.Alert.warning(x.title, x.responseText);
                            }
                        });
                    } else {
                        window.location.reload()
                    }
                }
            });
        });
    },
    LogOut: function () {
        var _this = this;
        _this.FacebookAppiSetUp();
        _this.GoogleAppiSetUp();
        $('#Logout').on('click', function(e) {
            e.preventDefault();
            var provider = $(this).data("provider");
            if (provider == "Facebook") {
                FB.getLoginStatus((response) => {
                    if (response.status === 'connected') {
                        FB.logout(function (response) {
                            console.log(response)
                            if (response.status == "unknown") {
                                $.AdminJs.Ajax.init({
                                    type: 'POST',
                                    url: '/en/User/LogOut/',
                                    data: {},
                                    action: (x) => {
                                        window.location.reload();
                                    }
                                });
                            }
                        });
                    }
                });
                
            } else if (provider == "Local") {
                $.AdminJs.Ajax.init({
                    type: 'POST',
                    url: '/en/User/LogOut/',
                    data: {},
                    action: (x) => {
                        window.location.reload();
                    }
                });
            } else if (provider == "Google") {
                var auth2 = gapi.auth2.getAuthInstance();
                auth2.signOut().then(function () {
                    $.AdminJs.Ajax.init({
                        type: 'POST',
                        url: '/en/User/LogOut/',
                        data: {},
                        action: (x) => {
                            window.location.reload();
                        }
                    });
                });
            }
        })
    },
    attachSignin: function (element) {
        var _this = this;
        auth2.attachClickHandler(element, {}, _this.onGoogleLoginSuccess, _this.onGoogleLoginFailure);
    },
    onGoogleLoginSuccess: function (user) {
        var _this = this;
        googleUser = user.getBasicProfile();
        console.log('Image URL: ' + googleUser.getImageUrl());
        $.AdminJs.Ajax.init({
            type: 'POST',
            url: '/en/User/GoogleLogIn/',
            data: { Id: googleUser.getId(), Name: googleUser.getName(), Email: googleUser.getEmail(), Photo: googleUser.getImageUrl() },
            action: (x) => {
                if (x.status == "info") {
                    var auth2 = gapi.auth2.getAuthInstance();
                    auth2.signOut().then(function () {
                        $.AdminJs.Alert.warning(x.title, x.responseText);
                    });
                } else {
                    window.location.reload()
                }
            }
        })
    },
    onGoogleLoginFailure: function (error) {
        console.log(error);
    },
    signinChanged: function (val) {
        console.log('Signin state changed to ', val);
    },
    sendGoogleLogIn: function (user) {
        if (user.getBasicProfile() != undefined) {
            googleUser = user.getBasicProfile();
            $.AdminJs.Ajax.init({
                type: 'POST',
                url: '/en/User/GoogleLogIn/',
                data: { Id: googleUser.getId(), Name: googleUser.getName(), Email: googleUser.getEmail(), Photo: googleUser.getImageUrl() },
                action: (x) => {
                    if (x.status == "info") {
                        var auth2 = gapi.auth2.getAuthInstance();
                        auth2.signOut().then(function () {
                            $.AdminJs.Alert.warning(x.title, x.responseText);
                        });
                    } else {
                        window.location.reload()
                    }
                }
            })
        }
    },
    refreshValues: function (val) {
        var _this = this;
        console.log('Signin state changed to ', val);
        if (auth2 && val) {
            console.log('Refreshing values...');

            googleUser = auth2.currentUser.get();

            console.log(googleUser.getBasicProfile());

            
        }
    },
    GoogleAppiSetUp: function (x) {
        var _this = this;
        var appStart = function () {
            gapi.load('auth2', initSigninV2);
        };

        var initSigninV2 = function () {
            auth2 = gapi.auth2.init({
                client_id: '338858308946-6vc4hpn2g48hmgjd94stkqf21a49trlh.apps.googleusercontent.com',
                cookiepolicy: 'single_host_origin',
                // Request scopes in addition to 'profile' and 'email'
                //scope: 'profile email name'
            });
            if (x == "beforeLogin") {
                _this.attachSignin(document.getElementById('GoogleLogInNavbar'));
                if (document.getElementById('GoogleLogIn') != undefined) {
                    _this.attachSignin(document.getElementById('GoogleLogIn'));
                }
                
                auth2.isSignedIn.listen(_this.refreshValues);
                auth2.currentUser.listen(_this.sendGoogleLogIn);
            }
            //auth2.isSignedIn.listen(_this.signinChanged);
            
            
        };
        appStart();
    },
    FacebookAppiSetUp: function (x) {
        var _this = this;
        window.fbAsyncInit = () => {
            FB.init({
                appId: '447263509253110',
                cookie: true,
                xfbml: true,
                version: 'v5.0'
            });
            if (x == "beforeLogin") {
                FB.getLoginStatus((response) => {
                    if (response.status === 'connected') {
                        _this.sendFacebookLogIn();
                    }
                });
            }
        };
    }
}

$.AdminJs.LogUp = {
    activate: function () {
        var _this = this;
        var markup = `
                <h5 class="mb-3">Datos de la empresa</h5>
                <div class="row mb-3">
                    <div class="col-sm-10 mb-3">
                        <div class="input-group">
                            <input class="form-control" data-val="true" data-val-length="RUT debe tener 12 dígitos." data-val-length-max="12" data-val-length-min="12" data-val-regex="RUT inválido. Favor seguir Ej: xx.xxx.xxx-x" data-val-regex-pattern="[0-9]{1,2}.[0-9]{3}.[0-9]{3}-[0-9Kk]{1}" data-val-required="Este campo es requerido." id="Company_CompanyId" name="Company.CompanyId" placeholder="Número de ID Ej: xx.xxx.xxx-x" type="text" value="">
                            <span class="input-group-addon">
                                <i class="far fa-id-card"></i>
                            </span>
                        </div>
                        <span class="field-validation-valid" data-valmsg-for="Company.CompanyId" data-valmsg-replace="true"></span>
                    </div>
                    <div class="col-sm-10 mb-3">
                        <div class="input-group">
                            <input class="form-control" data-val="true" data-val-required="Este campo es requerido." id="Company_CompanyName" name="Company.CompanyName" placeholder="Razón social" type="text" value="">
                            <span class="input-group-addon">
                                <i class="fas fa-user"></i>
                            </span>
                        </div>
                        <span class="field-validation-valid" data-valmsg-for="Company.CompanyName" data-valmsg-replace="true"></span>
                    </div>
                    <div class="col-sm-10 mb-3">
                        <div class="input-group">
                            <input class="form-control" data-val="true" data-val-required="Este campo es requerido." id="Company_CompanyActivity" name="Company.CompanyActivity" placeholder="Giro" type="text" value="">
                            <span class="input-group-addon">
                                <i class="fas fa-tags"></i>
                            </span>
                        </div>
                        <span class="field-validation-valid" data-valmsg-for="Company.CompanyActivity" data-valmsg-replace="true"></span>
                    </div>
                    <div class="col-sm-10 mb-3">
                        <div class="input-group">
                            <input class="form-control" data-val="true" data-val-required="Este campo es requerido." id="Company_CompanyPhone" name="Company.CompanyPhone" placeholder="9 XXXX XXXX Ej: 912345678" type="text" value="">
                            <span class="input-group-addon">
                                <i class="fas fa-phone-alt"></i> 
                            </span>
                        </div>
                        <span class="field-validation-valid" data-valmsg-for="Company.CompanyPhone" data-valmsg-replace="true"></span>
                    </div>
                    <div class="col-sm-10 mb-3">
                        <div class="input-group">
                            <select class="form-control" type="text" name="Company.CompanyStates" id="strCompanyStatesNationale" placeholder="Estado/Región" data-val="true" data-val-required="Este campo es requerido.">
                                <option value="">Estado/Región</option>

                            </select>
                            <span class="input-group-addon">
                                <i class="fas fa-flag"></i>
                            </span>
                        </div>
                        <span class="field-validation-valid" data-valmsg-for="Company.CompanyStates" data-valmsg-replace="true"></span>
                    </div>
                    <div class="col-sm-10 mb-3">
                        <div class="input-group">
                            <select class="form-control" type="text" name="Company.CompanyProvinces" id="strCompanyProvinciaNationale" placeholder="Provincia" data-val="true" data-val-required="Este campo es requerido.">
                                <option value="">Provincia</option>
                            </select>
                            <span class="input-group-addon">
                                <i class="fas fa-city"></i>
                            </span>
                        </div>
                        <span class="field-validation-valid" data-valmsg-for="Company.CompanyProvinces" data-valmsg-replace="true"></span>
                    </div>
                    <div class="col-sm-10 mb-3">
                        <div class="input-group">
                            <select class="form-control" type="text" name="Company.CompanyComunes" id="strCompanyComunaNationale" placeholder="Comuna" data-val="true" data-val-required="Este campo es requerido.">
                                <option value="">Comuna</option>
                            </select>
                            <span class="input-group-addon">
                                <i class="far fa-object-group"></i>
                            </span>
                        </div>
                        <span class="field-validation-valid" data-valmsg-for="Company.CompanyComunes" data-valmsg-replace="true"></span>
                    </div>
                    <div class="col-sm-10 mb-3">
                        <div class="input-group">
                            <input class="form-control" data-val="true" data-val-required="Este campo es requerido." id="Company_CompanyCity" name="Company.CompanyCity" placeholder="Ciudad" type="text" value="">
                            <span class="input-group-addon">
                                <i class="fas fa-landmark"></i>
                            </span>
                        </div>
                        <span class="field-validation-valid" data-valmsg-for="Company.CompanyCity" data-valmsg-replace="true"></span>
                    </div>
                    <div class="col-sm-10 mb-3">
                        <div class="input-group">
                            <input class="form-control" data-val="true" data-val-required="Este campo es requerido." id="Company_CompanyAddressOne" name="Company.CompanyAddressOne" placeholder="Dirección1" type="text" value="">
                            <span class="input-group-addon">
                                <i class="fas fa-map-marker-alt"></i>
                            </span>
                        </div>
                        <span class="field-validation-valid" data-valmsg-for="Company.CompanyAddressOne" data-valmsg-replace="true"></span>
                    </div>
                </div>`;
        $.AdminJs.Api.getRegions("#strStatesNationale");

        $('input[name="RegistrationType"]').on('change', function () {
            var value = $(this).val();
            if (value == 2) {
                $('#CompanyForm').html(markup);
                $('#LogUp').removeData('validator');
                $('#LogUp').removeData('unobtrusiveValidation');
                $.validator.unobtrusive.parse('#LogUp');
                $.AdminJs.Api.getRegions("#strCompanyStatesNationale");
                $('#CompanyForm').removeClass('d-none');

                $('#strCompanyStatesNationale').on('change', function () {
                    var id = $(this).val();
                    $.AdminJs.Api.getProvinces(id, "#strCompanyProvinciaNationale");
                });

                $('#strCompanyProvinciaNationale').on('change', function () {
                    var id = $(this).val();
                    $.AdminJs.Api.getComunes(id, "#strCompanyComunaNationale");
                });
            } else {
                $('#CompanyForm').addClass('d-none');
                $('#CompanyForm').html('');
            }
        });

        $('#LogUp').on('submit', function (e) {
            e.preventDefault();
            var x = $(this).serialize();
            if ($(this).valid()) {
                
                $.AdminJs.Ajax.init({
                    type: 'POST',
                    url: '/en/User/Registration/',
                    data: x,
                    action: function (e) {
                        if (e.status == "OK") {
                            $.AdminJs.Alert.success(e.title, e.responseText, "/");
                        }
                    }
                }, true);
            }
        });

        $('#strStatesNationale').on('change', function () {
            var id = $(this).val();
            $.AdminJs.Api.getProvinces(id, "#strProvinciaNationale");
        });

        $('#strProvinciaNationale').on('change', function () {
            var id = $(this).val();
            $.AdminJs.Api.getComunes(id, "#strComunaNationale");
        });
    }
}

$.AdminJs.compare = {
    activate: function () {
        var _this = this;
        $('.mg-product-wish').on('click', function (e) {
            e.preventDefault()
            var data = {
                Code: $(this).data('code'),
                Name: $(this).data('name'),
                Price: $(this).data('price'),
                PriceOff: $(this).data('off-price'),
                PercentageOff: $(this).data('off-percentage'),
                Quantity: $(this).data('quantity') == "" || $(this).data('quantity') == undefined ? $('select[name="Quantity"]').val() : $(this).data('quantity'),
                Category: $(this).data('category')
            }
            _this.sendAddProductToQuoting($.param(data));
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
    },
    sendAddProductToQuoting: (x) => {
        $.AdminJs.Ajax.init({
            type: 'POST',
            url: '/en/User/AddProductToQuoting/',
            data: x,
            action: (resp) => {
                if (resp.status == "OK") {
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
                        exit: 'animated fadeOutRight',
                        url: '/es/Usuario/Cotizar'
                    }
                    myAlert(data)
                } else {
                    $(this).removeClass('active');
                    var data = {
                        icon: 'far fa-times-circle',
                        title: '',
                        message: "No se puedo añadir este producto a la cotización.<br /> Intente nuevamente.",
                        type: 'danger',
                        from: 'top',
                        align: 'right',
                        mouse: 'pause',
                        enter: 'animated fadeInRight',
                        exit: 'animated fadeOutRight'
                    }
                    myAlert(data)
                }
            }
        })
    }
}

$.AdminJs.addresses = {
    activate: function () {
        var _this = this;
        $('.setDefault').on('click', function (e) {
            e.preventDefault();
            var id = $(this).data('id');

            $.AdminJs.Ajax.init({
                type: 'POST',
                url: '/en/Account/SetDefaultAddress/',
                data: { id: id },
                action: (resp) => {
                    if (resp.status == "OK") {
                        window.location.reload();
                    }
                }
            })
        })

        $('.setDelete').on('click', function (e) {
            e.preventDefault();
            var id = $(this).data('id');

            $.AdminJs.Ajax.init({
                type: 'POST',
                url: '/en/Account/SetDeleteAddress/',
                data: { id: id },
                action: (resp) => {
                    if (resp.status == "OK") {
                        window.location.reload();
                    }
                }
            })
        })
    }
}

$.AdminJs.editAddress = {
    activate: function () {
        var _this = this;
        $('input[name="ShippingType"]').on('change', function (e) {
            e.preventDefault();
            var id = $(this).val();
            if (id == 1) {
                $('#formDisplay .col-lg-4:lt(3)').hide();
                $('#formDisplay .col-lg-4:lt(3) input').attr('disabled', true);
            } else {
                $('#formDisplay .col-lg-4:lt(3)').show();
                $('#formDisplay .col-lg-4:lt(3) input').attr('disabled', false);
            }
            $('#formDisplay').removeClass('d-none');
        })
        $('#addNewNationalAddress').on('submit', function (e) {
            e.preventDefault();
            var form = $(this);
            if (form.valid()) {
                _this.sendEditNewAddress(form.serialize());
            }
        })
    },
    sendEditNewAddress: (x) => {
        $.AdminJs.Ajax.init({
            type: 'POST',
            url: '/en/Account/EditAddress/',
            data: x,
            action: (resp) => {
                if (resp.status == "OK") {
                    $.AdminJs.Alert.success(resp.title, resp.responseText, '/es/Cuenta/Direcciones');
                } else {
                    $.AdminJs.Alert.error(resp.title, resp.responseText);
                }
            }
        },true)
    }
}

$.AdminJs.addAddress = {
    activate: function () {
        var _this = this;
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
                url: '/en/api/Provinces/',
                data: { id: id },
                success: function (e) {
                    var modelsHtml = "<option value=''>Provincia</option>";
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
                url: '/en/api/Communes/',
                data: { id: id },
                success: function (e) {
                    var modelsHtml = "<option value=''>Comuna</option>";
                    $.each(e, function (a, b) {
                        modelsHtml += "<option value='" + b.id + "'>" + b.nombre + "</option>";
                    })
                    $("#strComunaNationale").html(modelsHtml);
                }
            })
        })

        $('#addNewNationalAddress').on('submit', function (e) {
            e.preventDefault();
            var form = $(this);
            if (form.valid()) {
                _this.sendAddNewAddress(form.serialize());
            }
        })
    },
    sendAddNewAddress: (x) => {
        $.AdminJs.Ajax.init({
            type: 'POST',
            url: '/en/Account/AddNewAddress/',
            data: x,
            action: (resp) => {
                if (resp.status == "OK") {
                    $.AdminJs.Alert.success(resp.title, resp.responseText, '/es/Cuenta/Direcciones');
                } else {
                    $.AdminJs.Alert.error(resp.title, resp.responseText);
                }
            }
        },true)
    }
}

$.AdminJs.cart = {
    active: function () {
        var _this = this;

        $('#EmptyQuotingList').on('click', function (e) {
            e.preventDefault();
            $.AdminJs.Ajax.init({
                type: 'POST',
                url: '/en/User/DeleteCartList/',
                data: "",
                action: (resp) => {
                    if (resp.status == "OK") {
                        window.location.reload();
                    }
                }
            })
        });

        $('.selectQuantity').on('change', function (e) {
            var id = $(this).data("id");
            var quantity = $(this).val();
            if (quantity == 7) {
                $(this).parent().find("input").attr("disabled", false);
                $(this).hide();
                $(this).parent().find(".form-group").removeClass("d-none");
            } else {
                _this.updateCartItemQuantity(id, quantity);
            }
        });

        $('input[name="quantityBox"]').on('focusout', function () {
            var quantity = $(this).val();
            var id = $(this).data('id');
            if (quantity < 7) {
                var select = $(this).parents('.count-input').find('select');
                $(this).parent().addClass("d-none");
                $(this).attr("disabled", true);
                select.val(quantity);
                select.find('option').each(function (i, val) {
                    if ($(this).val() != quantity) {
                        if ($(this).attr('selected')) {
                            $(this).attr("selected", false);
                        }
                    } else {
                        $(this).attr("selected", true);
                    }
                })

                select.show();
                _this.updateCartItemQuantity(id, quantity);
            } else {
                _this.updateCartItemQuantity(id, quantity);
            }
        });

        _this.removeFromCart();
    },
    addProductToCart: function() {
        var _this = this;
        $('#addProductToCart').on('click', function () {
            var data = {
                Code: $(this).data('code'),
                Name: $(this).data('name'),
                Price: $(this).data('price'),
                PriceInt: $(this).data('priceint'),
                PriceOff: $(this).data('off-price'),
                PriceOffInt: $(this).data('off-priceint'),
                PercentageOff: $(this).data('off-percentage'),
                Quantity: $(this).data('quantity') == "" || $(this).data('quantity') == undefined ? $('select[name="Quantity"]').val() : $(this).data('quantity'),
                Category: $(this).data('category')
            }
            _this.sendAddProductToCart($.param(data));
        })
    },
    sendAddProductToCart: (x) => {
        $.AdminJs.Ajax.init({
            type: 'POST',
            url: '/en/User/AddProductToCart/',
            data: x,
            action: (resp) => {
                if (resp.status == "OK") {
                    $(this).addClass('active');
                    var data = {
                        icon: 'far fa-check-circle',
                        title: '',
                        message: TranslateText("cartAdd"),
                        type: 'success',
                        from: 'top',
                        align: 'right',
                        mouse: 'pause',
                        enter: 'animated fadeInRight',
                        exit: 'animated fadeOutRight',
                        url: '/es/Usuario/Carrito'
                    }
                    myAlert(data)
                } else {
                }
            }
        })
    },
    updateCartItemQuantity: (x, y) => {
        $.AdminJs.Ajax.init({
            type: 'POST',
            url: '/en/User/UpdateItemQuantityFromCartList/',
            data: { code: x, quantity: y },
            action: (resp) => {
                window.location.reload();
            }
        })
    },
    removeFromCart: () => {
        $('.remove-from-cart').on('click', function () {
            var id = $(this).data('id');
            $.AdminJs.Ajax.init({
                type: 'POST',
                url: '/en/User/RemoveItemFromCartList/',
                data: { id: id },
                action: (resp) => {
                    if (resp.status == "OK") {
                        if (resp.reload == true) {
                            window.location.reload();
                        } else {
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
                        }
                    }
                }
            })
        });
        $('#EmptyCartList').on('click', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            $.AdminJs.Ajax.init({
                type: 'POST',
                url: '/en/User/DeleteCartList/',
                data: { id: id },
                action: (resp) => {
                    if (resp.status == "OK") {
                        if (resp.reload == true) {
                            window.location.reload();
                        } else {
                            $(this).parents('.entry')
                                .slideUp(function () {
                                    $(this).parents('.entry').remove();
                                });
                        }
                    }
                }
            })
        });
    }
}

$.AdminJs.quoting = {
    activate: function () {
        var _this = this;
        $('.remove-from-cart').on('click', function () {
            var id = $(this).data('id');
            $.AdminJs.Ajax.init({
                type: 'POST',
                url: '/en/User/RemoveItemFromQuotingList/',
                data: { id: id },
                action: (resp) => {
                    if (resp.status == "OK") {
                        if (resp.reload == true) {
                            window.location.reload();
                        } else {
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
                        }
                    }
                }
            })
        });

        $('#EmptyQuotingList').on('click', function (e) {
            e.preventDefault();
            $.AdminJs.Ajax.init({
                type: 'POST',
                url: '/en/User/DeleteQuotingList/',
                data: "",
                action: (resp) => {
                    if (resp.status == "OK") {
                        window.location.reload();
                    }
                }
            })
        });

        $('.selectQuantity').on('change', function (e) {
            var id = $(this).data("id");
            var quantity = $(this).val();
            if (quantity == 7) {
                $(this).parent().find("input").attr("disabled", false);
                $(this).hide();
                $(this).parent().find(".form-group").removeClass("d-none");
            } else {
                _this.updateQuotingItemQuantity(id, quantity);
            }
        });

        $('input[name="quantityBox"]').on('focusout', function () {
            var quantity = $(this).val();
            var id = $(this).data('id');
            if (quantity < 7) {
                var select = $(this).parents('.count-input').find('select');
                $(this).parent().addClass("d-none");
                $(this).attr("disabled", true);
                select.val(quantity);
                select.find('option').each(function (i, val) {
                    if ($(this).val() != quantity) {
                        if ($(this).attr('selected')) {
                            $(this).attr("selected", false);
                        }
                    } else {
                        $(this).attr("selected", true);
                    }
                })

                select.show();
                _this.updateQuotingItemQuantity(id, quantity);
            } else {
                _this.updateQuotingItemQuantity(id, quantity);
            }
        });

        $('#QuotingForm').submit(function (e) {
            e.preventDefault();
            var form = $(this);
            if (form.valid()) {
                $.AdminJs.Ajax.init({
                    type: 'POST',
                    url: '/en/User/Quoting',
                    data: form.serialize(),
                    action: (resp) => {
                        $.AdminJs.Alert.success(resp.title, resp.responseText);
                    }
                }, true)
            }
        });
    },
    updateQuotingItemQuantity: (x, y) => {
        $.AdminJs.Ajax.init({
            type: 'POST',
            url: '/en/User/UpdateItemQuantityFromQuotingList/',
            data: { code: x, quantity: y },
            action: (resp) => {

            }
        },true)
    }
}

$.AdminJs.checkOut = {
    active: function () {
        var _this = this;

        _this.checkoutSteps();
        _this.validateForm();
        $('#strStatesNationale').on('change', function () {
            var id = $(this).val();
            $.AdminJs.Ajax.init({
                type: 'GET',
                dataType: "JSON",
                url: '/en/api/Provinces/',
                data: { id: id },
                action: function (e) {
                    var modelsHtml = "<option value=''>Provincia</option>";
                    $.each(e, function (a, b) {
                        modelsHtml += "<option value='" + b.id + "'>" + b.nombre + "</option>";
                    })
                    $("#strProvinciaNationale").html(modelsHtml);
                }
            }, false, true)
        });

        $('#strProvinciaNationale').on('change', function () {
            var id = $(this).val();
            $.AdminJs.Ajax.init({
                type: 'GET',
                dataType: "JSON",
                url: '/en/api/Communes/',
                data: { id: id },
                action: function (e) {
                    var modelsHtml = "<option value=''>Comuna</option>";
                    $.each(e, function (a, b) {
                        modelsHtml += "<option value='" + b.id + "'>" + b.nombre + "</option>";
                    })
                    $("#strComunaNationale").html(modelsHtml);
                }
            }, false, true)
        });

        $('#strStatesNationaleAnother').on('change', function () {
            var id = $(this).val();
            $.ajax({
                type: 'GET',
                dataType: "JSON",
                url: '/en/api/Provinces/',
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
                url: '/en/api/Communes/',
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

$.AdminJs.userSettings = {
    activate: function () {
        var _this = this;
        $('#strStatesNationale').on('change', function () {
            var id = $(this).val();
            $.AdminJs.Ajax.init({
                type: 'GET',
                dataType: "JSON",
                url: '/en/Account/Provinces/',
                data: { id: id },
                action: function (e) {
                    var modelsHtml = "<option value=''>Provincia</option>";
                    $.each(e, function (a, b) {
                        modelsHtml += "<option value='" + b.id + "'>" + b.nombre + "</option>";
                    })
                    $("#strProvinciaNationale").html(modelsHtml);
                }
            },false,true)
        })

        $('#strProvinciaNationale').on('change', function () {
            var id = $(this).val();
            $.AdminJs.Ajax.init({
                type: 'GET',
                dataType: "JSON",
                url: '/en/Account/Communes/',
                data: { id: id },
                action: function (e) {
                    var modelsHtml = "<option value=''>Comuna</option>";
                    $.each(e, function (a, b) {
                        modelsHtml += "<option value='" + b.id + "'>" + b.nombre + "</option>";
                    })
                    $("#strComunaNationale").html(modelsHtml);
                }
            },false, true)
        })

        $('#perfilUpdate').on('submit', function (e) {
            e.preventDefault();
            var form = $(this);
            if (form.valid()) {
                var x = form.serialize()
                Swal.fire({
                    title: "Confirmación",
                    html: "Esta dirección será almacenada como predefinida para cuando realice compras o envíos<br /> Puede cambiar esta configuración en <strong>Direcciones</strong>",
                    type: "info",
                    confirmButtonText: 'OK',
                    confirmButtonClass: 'bg-success',
                    allowOutsideClick: false,
                    allowEscapeKey: false
                }).then((resp) => {
                    x += "&Default=" + resp.value;
                    _this.sendUserUpdateInfo(x);
                });
            }
        })
    },
    sendUserUpdateInfo: (x) => {
        $.AdminJs.Ajax.init({
            type: 'POST',
            url: '/en/Account/UpdateUserInfo/',
            data: x,
            action: (resp) => {
                $.AdminJs.Alert.success(resp.title, resp.responseText, window.location);
            }
        }, true)
    }
}

$.AdminJs.passwordChange = {
    activate: function (x) {
        var _this = this;
        $.AdminJs.reveal.activate();
        if (x.status == "OK") {
            $.AdminJs.Alert.info(x.title, x.responseText);
        } else if (x.status == "warning") {
            $.AdminJs.Alert.warning(x.title, x.responseText);
        } else {
            $.AdminJs.Alert.error(x.title, x.responseText);
        }

        $('#changePassword').on('submit', function(e){
            e.preventDefault();
            var form = $(this);
            if (form.valid()) {
                _this.sendPasswordChange(form.serialize());
            }
        })
    },
    sendPasswordChange: (x) => {
        console.log(x)
        $.AdminJs.Ajax.init({
            type: 'POST',
            url: '/en/Account/PasswordChange/',
            data: x,
            action: (resp) => {
                if (resp.status == "OK") {
                    $.AdminJs.Alert.success(resp.title, resp.responseText, '/es/Cuenta/Configuracion');
                } else {
                    $.AdminJs.Alert.error(resp.title, resp.responseText);
                }
            }
        },true)
    }
}

$.AdminJs.question = {
    activate: function () {
        var _this = this;
        $('#QuestionProductForm').on('submit', function (e) {
            e.preventDefault();
            var form = $(this).serialize();
            var code = $('#addProductToCart').data("code");
            if ($('textarea[name="Question"]').val() != "" || $('textarea[name="Question"]').val() != undefined) {
                _this.sendQuestionForm(form+"&Code="+code);
            }
        })
    },
    sendQuestionForm: (x) => {
        $.AdminJs.Ajax.init({
            type: 'POST',
            url: '/en/Product/AddQuestion/',
            data: x,
            action: (resp) => {
                if (resp.status == "OK") {
                    if (resp.isLoggedIn) {
                        $.AdminJs.Alert.success(resp.title, resp.responseText, window.location);
                    } else {
                        window.location.href = resp.redirectToAction;
                    }
                }
            }
        }, true)
    }
}

$.AdminJs.history = {
    activate: function () {

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
    $.AdminJs.Search.activate();
    
    $('[data-toggle="tooltip"]').tooltip();
    $('[data-toggle="popover"]').popover();
});

/*$('#loading-image').bind('ajaxStart', function () {
    $(this).show();
}).bind('ajaxStop', function () {
    $(this).hide();
});*/

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
            if ($.AdminJs.CheckDevice.init()) {
                if (prevScrollpos > currentScrollPos) {
                    document.getElementById("navbar").style.bottom = "0";
                } else {
                    document.getElementById("navbar").style.bottom = "-100px";
                }
            }
            prevScrollpos = currentScrollPos;
        });

		jQuery.fn.clickToggle = function (a, b) {
			function cb() { [b, a][this._tog ^= 1].call(this); }
			return this.on("click", cb);
        };

        $('.mg-fix-bugg').clickToggle(_this.addOnToggle, _this.removeOnToggle);

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
    },
    addOnToggle: () => {
        $('.mg-search-overlay').show();
        $('.mg-search').addClass('off-view');
        $('.mg-site-search').addClass('in-view');
        $('.icon-default').addClass('icon-default-hover');
        $('.icon-hover').addClass('icon-hover-hover');
    },
    removeOnToggle: () => {
        $('.mg-site-search').removeClass('in-view');
        $('.mg-search').removeClass('off-view');
        $('.icon-default').removeClass('icon-default-hover');
        $('.icon-hover').removeClass('icon-hover-hover');
        $('.mg-search-overlay').hide();
        $('#searchBoxWrapper').hide();
        $('input[name="search"]').val("");
        $('#searchBoxContainer').html("");
    }
}

$.AdminJs.Search = {
    activate: function () {
        var _this = this;
        var typingTimer;
        var doneTypingInterval = 1500; 
        var $input = $('input[name="search"]');
        var form = $('#SearchForm');
        $input.on('keyup', function () {
            clearTimeout(typingTimer);
            typingTimer = setTimeout(_this.doneTyping, doneTypingInterval)
        })

        $input.on('keydown', function () {
            clearTimeout(typingTimer);
        });

        form.on('submit', function (e) {
            e.preventDefault();
            if ($(this).find("input").val() != "") {
                window.location.href = "/es/Buscar/" + $(this).find("input").val();
            }
        })

    },
    doneTyping: () => {
        $('#searchBoxWrapper').fadeIn('fast');
        $('#searchBoxWrapper').css({
            overflow: 'hidden'
        });
        /*var $Html = "";

        for (var i = 0; i < 16; i++) {
            $Html += `<div class="mg-searchBoxContainer-item">
                                <a href="#">
                                    <i class="fab fa-sistrix"></i>
                                    <span>Element `+i+`</span>
                                </a>
                            </div>`;
        }

        $('#searchBoxContainer').html($Html);*/
        var $input = $('input[name="search"]');

        if ($input.val() != "") {
            $.ajax({
                type: 'POST',
                url: '/en/categories/GetSearchData/',
                data: { id: $input.val() },
                beforeSend: () => {
                    $('#loadingSearch').show();
                },
                success: (resp) => {
                    $('#searchBoxContainer').html(resp);
                },
                complete: () => {
                    $('#searchBoxWrapper').css({
                        overflow: 'auto'
                    });
                    $('#loadingSearch').hide();
                }
            })
        } else {
            $('#searchBoxWrapper').fadeOut('fast');
            $('#searchBoxContainer').html("");
            $('#searchBoxWrapper').css({
                overflow: 'hidden'
            });
        }
        
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
        var _this = this;
        $('.mg-profile-toggle').on('click', function () {
            $('.mg-profile').toggleClass('open');
        });

        $('#EditAvatar').on('click', function (e) {
            e.preventDefault();
            $('.mg-profile').toggleClass('open');
            $('#avatarModal').modal({
                backdrop: 'static',
                keyboard: false
            });
        });

        $('.avatar').on('click', function () {
            if (!$(this).hasClass('selected')) {
                $(this).toggleClass('active');
            }
            if ($('.avatar').not($(this)).hasClass('active')) {
                $('.avatar').not($(this)).removeClass('active')
            }
        });

        $("#AvatarFile").change(function () {
            var noneAvatar = $('#noneAvatarImg');
            if (noneAvatar.is(":Visible")) {
                noneAvatar.addClass("d-none");
            }
            _this.readURL(this);
        });

        $('#removeAvatarImg').click(() => {
            $.AdminJs.Ajax.init({
                type: 'POST',
                url: '/en/Account/RemoveAvatarImg/',
                data: {},
                action: (resp) => {
                    if (resp.status === "OK") {
                        window.location.reload();
                    }
                }
            })
        });

        $('#removeAvatarIcon').click(() => {
            $.AdminJs.Ajax.init({
                type: 'POST',
                url: '/en/Account/RemoveAvatarIcon/',
                data: {},
                action: (resp) => {
                    if (resp.status === "OK") {
                        window.location.reload();
                    }
                }
            })
        });

        $('#AvatarForm').on('submit', function (e) {
            e.preventDefault();
            var fileInput = $('#AvatarFile').get(0).files;
            var formdata = new FormData();
            formdata.append("AvatarFile", fileInput[0]);
            if ($(this).valid()) {
                _this.sendAvatarChangeImg(formdata);
            }
        });

        $('#changeAvatarIcon').click((e) => {
            e.preventDefault();
            var icon = $(".avatar.active");
            if (icon.length > 0) {
                var name = icon.data("id"), type = icon.data("type");
                _this.sendAvatarChangeIcon(name, type);
            }
        });

        $('#avatarModal').on('hidden.bs.modal', function (e) {
            var noneAvatar = $('#noneAvatarImg');
            $('#AvatarForm').trigger("reset");
            if (noneAvatar.hasClass("d-none")) {
                noneAvatar.removeClass("d-none");
            }
            $('#imagePreview').css('background-image', 'none');
        });
    },
    readURL: (input) => {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#imagePreview').css('background-image', 'url(' + e.target.result + ')');
                $('#imagePreview').hide();
                $('#imagePreview').fadeIn(650);
            }
            reader.readAsDataURL(input.files[0]);
        }
    },
    sendAvatarChangeImg: (x) => {
        $.AdminJs.Ajax.jsonupload({
            type: 'POST',
            url: '/en/Account/ChangeAvatarImg/',
            data: x,
            action: (resp) => {
                if (resp.status === "OK") {
                    window.location.reload();
                }
            }
        }, true);
    },
    sendAvatarChangeIcon: (x, y) => {
        $.AdminJs.Ajax.init({
            type: 'POST',
            url: '/en/Account/ChangeAvatarIcon/',
            data: {Icon: x, Type: y},
            action: (resp) => {
                if (resp.status === "OK") {
                    window.location.reload();
                }
            }
        }, true);
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
    activate: function () {

        var nonLinearSlider = document.getElementById('ui-range-slider');

        noUiSlider.create(nonLinearSlider, {
            connect: true,
            step: 1,
            orientation: 'horizontal',
            behaviour: 'tap',
            start: [$('.mg-price-range-slider').data('start-min'), $('.mg-price-range-slider').data('start-max')],
            range: {
                'min': [0],
                'max': [$('.mg-price-range-slider').data('start-max')]
            },
            format: wNumb({
                decimals: 0,
                thousand: '.',
                prefix: '$ ',
            })
        });

        var nodes = [
            document.getElementById('ui-range-value-min'),
            document.getElementById('ui-range-value-max')
        ];

        nonLinearSlider.noUiSlider.on('update', function (values, handle, unencoded, isTap, positions) {
            nodes[handle].innerHTML = values[handle];
        });
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

$.AdminJs.staticGallery = {
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

$.AdminJs.carouselGallery = {
    activate: function () {
        var _this = this;

        $('.gallery-wrapper a').on('click', function (e) {
            e.preventDefault();
            var galleryWrapper = $(this).parents('.gallery-wrapper');
            var container = _this.getContainerElements(galleryWrapper);
            console.log(container)
            var $pswp = $('.pswp')[0],
                options = {
                    index: galleryWrapper.find('.owl-item:not(.cloned) a').index(this),
                    bgOpacity: 0.85,
                    showHideOpacity: true
                };

            var gallery = new PhotoSwipe($pswp, PhotoSwipeUI_Default, container, options);
            gallery.init();
        })
    },
    getContainerElements: (x) => {
        var container = [];

        $(x).find('.owl-item').not('.cloned').each(function () {
            var $link = $(this).find('a'),
                item = {
                    src: $link.attr('href'),
                    w: $link.data('width'),
                    h: $link.data('height'),
                    title: $(this).find('.caption').text()
                };
            container.push(item);
        });

        return container;
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

$.AdminJs.Alert = {
    success: function (x, r, y = false) {
        Swal.fire({
            title: x,
            html: r,
            type: "success",
            confirmButtonText: 'Ok',
            confirmButtonClass: 'bg-success',
        }).then(function () {
            if (y) {
                window.location.href = y;
            } else {
                window.location.reload();
            }
        });
    },
    warning: function (x, r) {
        Swal.fire({
            title: x,
            html: r,
            type: "warning",
            confirmButtonText: 'Cerrar',
            confirmButtonClass: 'bg-warning',
        })
    },
    info: function (x, r) {
        Swal.fire({
            title: x,
            html: r,
            type: "info",
            confirmButtonText: 'Ok',
            confirmButtonClass: 'bg-info',
        })
    },
    error: function (x, r) {
        Swal.fire({
            title: x,
            html: r,
            type: "error",
            confirmButtonText: 'Cerrar',
            confirmButtonClass: 'bg-danger',
        })
    }
}

$.AdminJs.Ajax = {
    init: (x, loading = false, partial = false) => {
        $.ajax({
            type: x.type,
            url: x.url,
            data: x.data,
            cache: false,
            beforeSend: function () {
                if (loading) {
                    $.AdminJs.Loading.start();
                }
            },
            success: function (e) {
                if (e.status == "OK" || partial || e.status == "info") {
                    x.action(e);
                } else if (e.status == "error") {
                    console.log(e)
                    $.AdminJs.Alert.error(e.title, e.responseText);
                } else if (e.status == "warning") {
                    $.AdminJs.Alert.warning(e.title, e.responseText);
                } else {
                    $('#errorReport').html(e.title, e.responseText);
                }
            },
            complete: function () {
                if (loading) {
                    $.AdminJs.Loading.stop();
                }
            },
            error: function (e) {
                console.log(e)
                $('#errorReport').html(e.responseText);
            }
        })
    },
    jsonupload: (x, loading = false, partial = false) => {
        $.ajax({
            type: x.type,
            url: x.url,
            data: x.data,
            cache: false,
            contentType: false,
            processData: false,
            beforeSend: function () {
                if (loading) {
                    $.AdminJs.Loading.start();
                }
            },
            success: function (e) {
                if (e.status == "OK" || partial || e.status == "info") {
                    x.action(e);
                } else if (e.status == "error") {
                    console.log(e)
                    $.AdminJs.Alert.error(e.title, e.responseText);
                } else if (e.status == "warning") {
                    $.AdminJs.Alert.warning(e.title, e.responseText);
                } else {
                    $('#errorReport').html(e.title, e.responseText);
                }
            },
            complete: function () {
                if (loading) {
                    $.AdminJs.Loading.stop();
                }
            },
            error: function (e) {
                console.log(e)
                $('#errorReport').html(e.responseText);
            }
        });
    }
}

$.AdminJs.Loading = {
    start: function () {
        $('body').css({ overflow: 'hidden' });
        $('#page-loader-wrapper').fadeIn('slow');
    },
    stop: function () {
        $('#page-loader-wrapper').fadeOut();
        $('body').css({ overflow: 'auto' });
    }
}

$.AdminJs.reveal = {
    activate: function () {
        $('.revealPass').on('click', function (e) {
            var element = $(this).children('i');
            if (element.hasClass('far fa-eye')) {
                element.removeClass('far fa-eye').addClass('far fa-eye-slash');
                $(this).parent('.form-group').find('input').attr('type', 'text');
            } else {
                element.removeClass('far fa-eye-slash').addClass('far fa-eye');
                $(this).parent('.form-group').find('input').attr('type', 'password');
            }
        })
    }
}

$.AdminJs.CheckDevice = {
    init: function () {
        var check = false;
        (function (a) { if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk/i.test(a) || /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0, 4))) check = true; })(navigator.userAgent || navigator.vendor || window.opera);
        return check;
    }
}

$.AdminJs.Api = {
    getRegions: (x) => {
        $.AdminJs.Ajax.init({
            type: 'GET',
            dataType: "JSON",
            url: '/en/api/States/',
            data: {},
            action: function (e) {
                var modelsHtml = "<option value=''>Estado/Región</option>";
                $.each(e, function (a, b) {
                    modelsHtml += "<option value='" + b.id + "'>" + b.nombre + " - " + b.number + "</option>";
                })
                $(x).html(modelsHtml);
            }
        }, false, true);
    },
    getProvinces: (x, y) => {
        $.AdminJs.Ajax.init({
            type: 'GET',
            dataType: "JSON",
            url: '/en/api/Provinces/',
            data: { id: x },
            action: function (e) {
                var modelsHtml = "<option value=''>Provincia</option>";
                $.each(e, function (a, b) {
                    modelsHtml += "<option value='" + b.id + "'>" + b.nombre + "</option>";
                })
                $(y).html(modelsHtml);
            }
        }, false, true);
    },
    getComunes: (x, y) => {
        $.AdminJs.Ajax.init({
            type: 'GET',
            dataType: "JSON",
            url: '/en/api/Communes/',
            data: { id: x },
            action: function (e) {
                var modelsHtml = "<option value=''>Comuna</option>";
                $.each(e, function (a, b) {
                    modelsHtml += "<option value='" + b.id + "'>" + b.nombre + "</option>";
                })
                $(y).html(modelsHtml);
            }
        }, false, true);
    }
}

function myAlert(x) {
	$.notify({
		icon: x['icon'],
		title: x['title'],
        message: x['message'],
        url: x.url,
        target: '_self'
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

function onSignIn(googleUser) {
    var profile = googleUser.getBasicProfile();
    console.log('ID: ' + profile.getId()); // Do not send to your backend! Use an ID token instead.
    console.log('Name: ' + profile.getName());
    console.log('Image URL: ' + profile.getImageUrl());
    console.log('Email: ' + profile.getEmail()); // This is null if the 'email' scope is not present.
};

function signOut() {
    var auth2 = gapi.auth2.getAuthInstance();
    auth2.signOut().then(function () {
        console.log('User signed out.');
    });
}