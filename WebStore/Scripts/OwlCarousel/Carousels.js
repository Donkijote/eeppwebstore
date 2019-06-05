$(function () {
    $("#carouselMain").owlCarousel({
        items: 1,
        nav: true,
        dots: true,
        navText: ["<i class='fas fa-angle-left fa-2x'></i>", "<i class='fas fa-angle-right fa-2x'></i>"],
        loop: true,
        margin: 0,
        responsiveClass: true
    });

    $("#carouselPopularBrands").owlCarousel({
        items: 6,
        nav: false,
        dots: false,
        navText: ["<i class='fas fa-angle-left fa-2x'></i>", "<i class='fas fa-angle-right fa-2x'></i>"],
        loop: true,
        margin: 10,
        responsiveClass: true,
        responsive: {
            0: {
                items: 1
            },
            600: {
                items: 1
            },
            1000: {
                items: 6
            }
        }
    });

    $("#carouselProductsImg").owlCarousel({
        items: 1,
        loop: true,
        margin: 10,
        nav: false,
        dots: false,
        responsiveClass: true,
        thumbs: true,
        thumbImage: false,
        thumbsPrerendered: true,
        thumbContainerClass: 'owl-thumbs',
        thumbItemClass: 'owl-thumb-item',
        responsive: {
            0: {
                items: 1
            },
            600: {
                items: 1
            },
            1000: {
                items: 1
            }
        }
    });

    $("#carouselYouMayLikeToo").owlCarousel({
        items: 4,
        nav: true,
        dots: true,
        navText: ["<i class='fas fa-angle-left fa-2x'></i>", "<i class='fas fa-angle-right fa-2x'></i>"],
        loop: true,
        margin: 10,
        responsiveClass: true,
        responsive: {
            0: {
                items: 1
            },
            600: {
                items: 2
            },
            1000: {
                items: 4
            }
        }
    });

    $("#carouselCategories").owlCarousel({
        items: 3,
        nav: true,
        dots: false,
        navContainerClass: "owl-nav outside-nav dark-theme",
        navText: ["<i class='fas fa-angle-left fa-2x'></i>", "<i class='fas fa-angle-right fa-2x'></i>"],
        loop: true,
        margin: 10,
        responsiveClass: true,
        responsive: {
            0: {
                items: 1
            },
            600: {
                items: 1
            },
            1000: {
                items: 3
            }
        }
    });
})