﻿'use strict';
mainApp.controller('transactionInfoBonusController',
[
    '$scope', '$state', 'transactionService', 'tradeService', 'Notification', 'TransactionType', 'TransactionState',
    'ConfigData',
    function($scope,
        $state,
        transactionService,
        tradeService,
        Notification,
        TransactionType,
        TransactionState,
        ConfigData) {

        $scope.loadData = function(tableState) {
            if (tableState) {
                $scope.tableState = tableState;
            } else {
                tableState = $scope.tableState;
            }
            $scope.data = [];
            transactionService.queryUserBonus(JSON.stringify(tableState),
                function(response) {
                    console.log(response);
                    $scope.data = response.data;
                    tableState.pagination.numberOfPages = Math.ceil(response.totalItems / tableState.pagination.number);
                    loadTransaction($scope.data);
                });
        }

        function loadTransaction(transactions) {
            _.each(transactions,
                function(item) {
                    item.isGiving = item.currentAccount == item.giverId;
                    item.isReceiving = item.currentAccount == item.receiverId;

                    item.isAllowConfirmGave = item.isGiving && item.state == TransactionState.Begin;
                    item.isAllowConfirmReceived = item.isReceiving && item.state == TransactionState.Transfered;

                    item.isAllowAbandonTransaction = false;

                    item.isAllowAttachment = item.state != TransactionState.Abandoned;
                    item.isAllowUploadAttachment = item.isGiving &&
                    (item.state == TransactionState.Begin || item.state == TransactionState.Transfered);

                    item.isAbandoned = item.state == TransactionState.Abandoned;
                });
        }

        $scope.reload = function() {
            $scope.loadData();
        }

        $scope.viewDefails = function(target) {
            if ($scope.current) {
                $scope.current.selected = false;
            }
            target.selected = true;
            $scope.current = target;

            $state.go('transaction.bonus.details', { key: target.id });
        }

        function updateAccountStatus() {
            tradeService.status({},
                function(response) {
                    $scope.info = response;
                });
        }

        $scope.claimBonus = function() {
            $scope.info.isAllowClaimBonus = false;
            tradeService.claimBonus({ key: $scope.info.userName },
                { key: $scope.info.userName },
                function(response) {
                    Notification.success('Claim bonus successful!');
                    updateAccountStatus();
                },
                function(response) {
                    $scope.info.isAllowClaimBonus = true;
                    Notification.success(response);
                });
        }

        $scope.init = function() {
            $scope.ConfigData = ConfigData;
            $scope.TransactionType = TransactionType;
            $scope.TransactionState = TransactionState;

            $scope.data = [];

            updateAccountStatus();
        }

        $scope.init();
    }
]);