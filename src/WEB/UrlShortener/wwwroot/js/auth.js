window.authHelpers = {
    setToken: function (token) {
        document.cookie = 'auth_token=' + token + '; path=/; SameSite=Strict';
    },
    getToken: function () {
        var match = document.cookie.match(/(^|;)\s*auth_token=([^;]+)/);
        return match ? match[2] : null;
    },
    clearToken: function () {
        document.cookie = 'auth_token=; path=/; expires=Thu, 01 Jan 1970 00:00:00 UTC; SameSite=Strict';
    }
};
