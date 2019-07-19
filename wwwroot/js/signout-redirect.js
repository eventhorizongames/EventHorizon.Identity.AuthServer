window.addEventListener('load', function() {
    var postLogoutRedirectUriLink = document.querySelector(
        'a.PostLogoutRedirectUri'
    );
    if (postLogoutRedirectUriLink) {
        window.setTimeout(function() {
            window.location = postLogoutRedirectUriLink.href;
        }, 500);
    }
});
