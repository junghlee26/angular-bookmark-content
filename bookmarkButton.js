(function () {

    var app = angular.module("BlakWealth");

    app.component("bookmarkToggle", {
        templateUrl: "bookmark-toggle/button.html",
        controller: "bookmarkController as vm",
        bindings: {
            contentId: '<',
            onRemove: '&'
        }
    });
})();


(function () {
    var app = angular.module('BlakWealth');

    app.service('bookmarkService', bookmarkService);

    bookmarkService.$inject = ['$http', '$q', '$timeout'];


    function bookmarkService($http, $q, $timeout) {
        var pendingIds = [];
        var deferred = null;

        // GET - checking bookmark status (contentIds by multiplexing)
        function _getStatusByContentIds(contentIds) {
            pendingIds.push(contentIds);
            if (!deferred) {
                deferred = $q.defer();
                $timeout(function () {
                    deferred.resolve(
                        $http({
                            url: '/api/bookmark/check',
                            data: { contentIds: pendingIds },
                            method: 'POST'
                        })
                    );
                    deferred = null;
                    pendingIds = [];
                });
            }

            return deferred.promise.then(response => response.data.item[contentIds]);
        }

        // POST - pass data (content id)
        function _addBookmark(data) {
            var settings = {
                url: '/api/bookmark',
                method: 'POST',
                cache: false,
                contentData: 'application/json; charset=UFT-8',
                data: data
            };
            return $http(settings);
        }

        // DELETE - remove bookmark (content id)
        function _removeBookmark(contentId) {
            var settings = {
                url: '/api/bookmark/' + contentId,
                method: 'DELETE',
                contentData: 'json/application; charset=UFT-8'
            };
            return $http(settings);
        }

        return {
            getStatusByContentIds: _getStatusByContentIds,
            addBookmark: _addBookmark,
            removeBookmark: _removeBookmark
        };
    }
})();


(function () {
    var app = angular.module('BlakWealth');

    app.controller('bookmarkController', bookmarkController);

    bookmarkController.$inject = ['bookmarkService', 'userService', '$uibModal'];


    function bookmarkController(bookmarkService, userService, $uibModal) {
        var vm = this;
        vm.bookmarks = [];
        vm.$onChanges = _init;
        vm.btnBookmark = _btnBookmark;


        function _init() {
            userService.getInfo().then(_getUserSuccess, _getError);
            bookmarkService.getStatusByContentIds(vm.contentId).then(_checkSuccess, _checkError);

        }


        function _checkSuccess(response) {
            if (response) {
                vm.bookmarked = true;
            }
            else {
                vm.bookmarked = false;
            }
        }

        function _checkError() {
            vm.bookmarked = false;
        }

        function _getUserSuccess(data) {
            if (data) {
                vm.signedIn = true;
            }
            else {
                vm.signedIn = false;
            }
        }

        function _getError() {
            console.log("getting user info error");
        }


        //add or remove content by button status (on/off)
        function _btnBookmark() {

            if (vm.signedIn == true) {
                if (!vm.bookmarked) {
                    vm.data = {};
                    vm.data.contentId = vm.contentId;
                    bookmarkService.addBookmark(vm.data).then(_addSuccess, _addError);
                }
                else {
                    $uibModal.open({
                        templateUrl: 'modal.html',
                        controller: modalPopupController
                    });

                    modalPopupController.$inject = ['$scope', '$uibModalInstance']


                    function modalPopupController($scope, $uibModalInstance) {
                        $scope.btnRemove = function () {
                            bookmarkService.removeBookmark(vm.contentId).then(_removeSuccess, _removeError);
                            $uibModalInstance.close();
                        };
                        $scope.btnCancel = function () {
                            $uibModalInstance.close();
                        };
                    }

                }
            } else {
                function _btnSignIn() {
                    modalController.openModal();
                }
            }

        }

        function _addSuccess(response) {
            vm.bookmarked = true;
            console.log(response);
        }

        function _addError() {
            vm.bookmarked = false;
            console.log("bookmark error");
        }


        function _removeSuccess(response) {
            vm.bookmarked = false;
            console.log(response);
            vm.onRemove();
        }

        function _removeError() {
            vm.bookmarked = true;
            console.log("remove bookmark error");
        }

    }
})();