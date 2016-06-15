'use strict';
mainApp.controller('transactionInfoDetailsController',
[
    '$scope', '_', '$stateParams', '$timeout', '$uibModal', 'transactionService',
    'Notification', 'ConfigData', 'AccountState', 'TransactionState',
    function($scope,
        _,
        $stateParams,
        $timeout,
        $uibModal,
        transactionService,
        Notification,
        ConfigData,
        AccountState,
        TransactionState) {

        $scope.getData = function() {
            transactionService.transactionDetails({ key: $stateParams.key },
                { key: $stateParams.key },
                function(response) {
                    loadTransaction(response);
                });
        }

        function loadTransaction(item) {
            $scope.target = item;

            item.isGiving = item.currentAccount == item.giverId;
            item.isReceiving = item.currentAccount == item.receiverId;

            item.isAllowConfirmGave = item.isGiving && item.state == TransactionState.Begin;
            item.isAllowConfirmReceived = item.isReceiving && item.state == TransactionState.Transfered;

            item.isAllowAbandonTransaction = false;

            item.isAllowAttachment = item.state != TransactionState.Abandoned;
            item.isAllowUploadAttachment = item.isGiving &&
            (item.state == TransactionState.Begin || item.state == TransactionState.Transfered);

            item.isAbandoned = item.state == TransactionState.Abandoned;


        }

        $scope.moneyTransfered = function(target) {
            $scope.isProcessing = true;
            transactionService.moneyTransfered(target,
                function(response) {
                    Notification.success('Money Transfered');
                    $scope.getData();
                    $scope.isProcessing = false;
                });
        }

        $scope.moneyReceived = function(target) {
            $scope.isProcessing = true;
            transactionService.moneyReceived(target,
                function(response) {
                    Notification.success('Money Received');
                    $scope.getData();
                    $scope.isProcessing = false;
                });
        }

        $scope.reportNotTransfer = function(target) {
            $scope.isProcessing = true;
            transactionService.reportNotTransfer(target,
                function(response) {
                    Notification.success('Transaction Reported');
                    $scope.getData();
                    $scope.isProcessing = false;
                });
        }

        $scope.updateImg = function(target) {
            console.log('---');
            transactionService.updateImg(target,
                function(response) {
                    Notification.success('Upload saved.');
                });
        }

        $scope.upload = function(target) {
            var modalInstance = $uibModal.open({
                templateUrl: 'app/main/transaction/importPopup.html',
                size: 'lg',
                controller: 'importPopupController',
                resolve: {
                    targetData: function() {
                        return { uploadLink: 'api/transaction/upload' };
                    }
                }
            });

            modalInstance.result.then(function(returnData) {
                    Notification.success('Upload successful.');
                    target.attachmentUrl = returnData;
                    $scope.updateImg(target);
                },
                function() {
                    Notification.success('Upload error.');
                });
        };

        $scope.abandon = function(target) {
            $scope.isProcessing = true;
            transactionService.abandonTransaction(target,
                function(response) {
                    Notification.success('Transaction Abandoned');
                    $scope.getData();
                    $scope.isProcessing = false;
                });
        }


        $scope.init = function() {
            $scope.TransactionState = TransactionState;
            $scope.AccountState = AccountState;
            $scope.ConfigData = ConfigData;

            $scope.isProcessing = false;
            $scope.selected = true;

            $scope.getData();
        }

        $scope.init();

    }
]);