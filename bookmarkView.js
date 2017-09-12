(function () {

    var app = angular.module('BlakWealth');
    
    app.config(configure);
    configure.$inject = ['$stateProvider'];
    function configure($stateProvider) {
        $stateProvider.state('bookmark', {
            name: 'bookmark',
            component: 'bookmarkList',
            url: '/bookmark'
        });
    }


    app.component('bookmarkList', {
        templateUrl: 'bookmark-view/bookmark-component.html',
        controller: 'bookmarkViewController as bc',
        bindings: {
            contentId: '<',
            getContentInfo: '&',
            likesCount: '&'
        }
    })
})();



(function () {
    var app = angular.module("BlakWealth");
    app.controller('bookmarkViewController', bookmarkViewController);
    bookmarkViewController.$inject = ['bookmarkViewService', 'userService', '$uibModal', '$state'];

    function bookmarkViewController(bookmarkViewService, userService, $uibModal, $state) {
        var bc = this;
        bc.bookmarks = [];
        bc.index = null;
        bc.search = '';
        bc.busy = false;
        bc.bookmarkList = false;

        bc.more = {
            itemNum: 15,
            pageNum: 0
        };

        bc.loadMore = _init;


        function _init() {
            if (bc.busy) return;
            bc.busy = true;
            bookmarkViewService.getAllBookmarks(bc.more).then(_getSuccess, _getError);
        }

        function _getSuccess(response) {
            if (response.data.items != null) {
                bc.bookmarkList = true;
                for (var i = 0; i < response.data.items.length; i++) {
                    var contents = response.data.items[i];
                    bc.bookmarks.push(contents);
                }
                bc.more.pageNum = bc.more.pageNum + 1;
                bc.busy = false;
            } else {
                bc.busy = true;
            }
        }

        function _getError() {
            console.log("Getting bookmark contents Error");
            bc.busy = true;
        }

        //this will remove un-bookmarked item from user's bookmark page
        bc.remove = function (index) {
            bc.bookmarks.splice(index, 1);
        };
    }
})();

