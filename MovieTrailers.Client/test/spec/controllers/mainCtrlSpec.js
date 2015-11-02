describe("mainCtrl", function () {
    var $rootScope,
        $q,
        createController,
        trailerService,
        notificationService;

    beforeEach(function () {
        module('movieTrailersApp');
    });

    beforeEach(inject(function ($injector) {
        $rootScope = $injector.get('$rootScope');
        var $controller = $injector.get('$controller');
        $q = $injector.get('$q');

        trailerService = {
            
        };
        notificationService = {};
        $rootScope.query = "q";
        createController = function () {
            return $controller('mainCtrl', { '$scope': $rootScope, 'trailersService': trailerService, $mdDialog: {}, 'notificationService': notificationService });
        };
    }));

    it('isLoading true when search in process', function () {
        trailerService.search = function () {
            var defer = $q.defer();
            defer.resolve([]);
            return defer.promise;
        }
        createController();
        expect($rootScope.isLoading).toBe(false);
        $rootScope.search({ keyCode: 13 });
        expect($rootScope.isLoading).toBe(true);
        $rootScope.$digest();
        expect($rootScope.isLoading).toBe(false);
    });

    it('when nextPage called currentPage increase', function () {
        trailerService.search = function () { return { then: function () { } }; };
        createController();
        expect($rootScope.currentPage).toBe(0);
        $rootScope.nextPage();
        expect($rootScope.currentPage).toBe(1);
    });

    it('when nextPage called currentPage not change if last', function () {
        trailerService.search = function () { return { then: function () { } }; };
        createController();
        $rootScope.totalResult = 40;
        $rootScope.currentPage = 1;
        $rootScope.nextPage();
        expect($rootScope.currentPage).toBe(1);
    });

    it('when prevPage called currentPage decrease', function () {
        trailerService.search = function () { return { then: function () { } }; };
        createController();
        $rootScope.currentPage = 2;
        $rootScope.prevPage();
        expect($rootScope.currentPage).toBe(1);
    });

    it('when prevPage called currentPage not change if first', function () {
        trailerService.search = function () { return { then: function () { } }; };
        createController();
        $rootScope.currentPage = 0;
        $rootScope.prevPage();
        expect($rootScope.currentPage).toBe(0);
    });

    it('getLastPageIndex should return zero-base last page index', function () {
        createController();
        $rootScope.totalResult = 45;
        expect($rootScope.getLastPageIndex()).toBe(2);
    });

    describe('when query empty', function () {
        beforeEach(function () {
            trailerService.search = jasmine.createSpy('search');
            notificationService.info = jasmine.createSpy('info');
            $rootScope.query = "";
            createController();
            $rootScope.search({ keyCode: 13 });
        });

        it('search is not called', function () {
            expect(trailerService.search.calls.any()).toBe(false);
        });

        it('notification info is called', function () {
            expect(notificationService.info).toHaveBeenCalled();
        });
    }); 

    it('when year changes search is called', function () {
        trailerService.search = jasmine.createSpy('search').and.returnValue({ then: function () { } });
        createController();
        $rootScope.selectedYear = 2015;
        $rootScope.$digest();
        expect(trailerService.search).toHaveBeenCalled();
    });
});