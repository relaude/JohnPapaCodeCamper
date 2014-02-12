define(['durandal/system',
        'durandal/plugins/router',
        'services/logger',
        'durandal/app',
        'services/appsecurity',
        'services/datacontext',
        'services/errorhandler', 
        'config'],
    function (system, router, logger, app, appsecurity, datacontext, errorhandler, config) {

        var adminRoutes = ko.computed(function () {
            return router.allRoutes().filter(function (r) {
                return r.settings.admin;
            });
        });

        var shell = {
            activate: activate,
            addSession: addSession,
            router: router,
            adminRoutes: adminRoutes,
            appsecurity: appsecurity,
            logout: function () {
                var self = this;
                appsecurity.logout().fail(self.handlevalidationerrors);
            }
        };

        errorhandler.includeIn(shell);

        return shell;

        //#region Internal Methods
        function activate() {
            app.title = config.appTitle;
            return datacontext
                .primeData()
                .then(boot)
                .fail(failedInitialization);
        }

        function boot() {
            logger.log('CodeCamper JumpStart Loaded!', null, system.getModuleId(shell), true);
            router.map(config.routes);
            return router.activate(config.startModule);
        }

        function addSession(item) {
            router.navigateTo(item.hash);
        }

        function failedInitialization(error) {
            var msg = 'App initialization failed: ' + error.message;
            logger.logError(msg, error, system.getModuleId(shell), true);
        }

        function log(msg, data, showToast) {
            logger.log(msg, data, system.getModuleId(shell), showToast);
        }
        //#endregion
    });