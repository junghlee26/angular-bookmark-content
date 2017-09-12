(function () {
    var app = angular.module("BlakWealth");

    app.service('bookmarkViewService', bookmarkViewService);

    bookmarkViewService.$inject = ['$http'];

    function bookmarkViewService($http) {

        // GET - get all bookmark by user id
        function _getAllBookmarks(more) {
            var settings = {
                url: '/api/bookmark/user?itemNum=' + more.itemNum + '&pageNum=' + more.pageNum,
                method: 'GET',
                cache: false,
                withCredentials: true
            };
            return $http(settings);
        }

        return {
            getAllBookmarks: _getAllBookmarks
        };
    }
})();