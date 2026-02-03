'use strict';

var optimize = false;
var gulp = require('gulp');
var watch = require('gulp-watch');
var batch = require('gulp-batch');
var path = require('path');

var mainBowerFiles = require('main-bower-files');
var bundledModFile = "vendor.js";
var bundledModFileAppData = "app.js";
var select2Index = 1;


var authTokenName = {
    development: 'dev-makalu-auth-token',
    stage: 'stage-makalu-auth-token',
    test: 'test-makalu-auth-token',
    production: 'prod-makalu-auth-token'
};

var tokenName = authTokenName.development;

var $ = require('gulp-load-plugins')(
    {
        pattern: ['gulp-*', 'main-bower-files', 'uglify-save-license', 'del']
    });

var paths = {
    dev: 'public',
    dep: 'client',
    tmp: 'tmp'
};

gulp.paths = paths;

gulp.task('styles', function () {
        
    var proc = gulp.src(path.join(paths.dev, '/content/styles/*.css'));

    if (optimize == true)
        proc = proc.pipe($.cleanCss({ compatibility: 'ie8' })); // for task prod

    return proc.pipe(gulp.dest(paths.dep + "/content/styles/"));
});


gulp.task('images', function () {
    return gulp.src(paths.dev + '/content/images/**/*')
        .pipe(gulp.dest(paths.dep + '/content/images/'));
});

gulp.task('scripts', function () {

    var jsFilter = $.filter([paths.dev + '/modules/**/*.js',
        '!' + paths.dev + '/modules/*.js',
        '!' + paths.dev + '/modules/*/*.js',
        '!' + paths.dev + '/modules/*/providers/*.js',
        '!' + paths.dev + '/modules/*/config/*.js',
        '!' + paths.dev + '/modules/core/impl/lookup.helper.js'], { restore: true });

    var proc = gulp.src(path.join(paths.dev, '/modules/**/*.js'));
        //.pipe($.eslint())
        //.pipe($.eslint.format())
        //.pipe($.size());

    if (optimize == true) {
        proc = proc
            .pipe($.uglify())
            .pipe(jsFilter)
            .pipe($.concat(bundledModFileAppData))
            .pipe(jsFilter.restore);
    }

    return proc.pipe(gulp.dest(paths.dep + "/modules/"));
});

gulp.task('scriptsLib', function () {

    var ignoreFiles = [/foundation\.css/, /material-design-iconic-font\.css/, /default\.css/];
    var jsFilter = $.filter(['public/lib/**/*.js'], { restore: true });
    var cssFilter = $.filter('public/lib/**/*.css', { restore: true });
    var fontsFilter = $.filter(['public/lib/**/*.eot', 'public/lib/**/*.svg', 'public/lib/**/*.ttf', 'public/lib/**/*.woff', 'public/lib/**/*.woff2',
        'public/content/fonts/**/*.*'], { restore: true });

    //console.log(jsFilter);

    var files = mainBowerFiles({
        paths: {
            //bowerDirectory: 'app/lib',
            bowerrc: '.bowerrc',
            bowerJson: 'bower.json'
        },
        filter: function (filePath) {
            for (var i = 0; i < ignoreFiles.length; i++) {
                if (filePath.indexOf(ignoreFiles[i]) !== -1)
                    return false;
            }
            return true;
        }
    });

    files.push(paths.dev + "/content/scripts/*.js");
    files.push(paths.dev + "/content/fonts/**/*.*");
    
    var proc = gulp.src(files)
        .pipe($.rename(function (path) {
            if (path.basename.indexOf('select2') >= 0 && path.extname == ".js") {
                path.basename += '-file-' + select2Index;
                select2Index++;
            }
        }))
        .pipe(jsFilter);

    if (optimize == true) {
        console.log("in optimiZe");
        proc = proc
            .pipe($.order([
                "jquery.js",
                "angular.js",
                "angular-translate.js",
                "inputmask.js",
                "jquery.inputmask.js",
                "moment.js",
                "vendor.js",
                "metronic.js",
                "bootstrap-hover-dropdown.js",
                "aes.js",
                "angularjs-crypto.js",
                "CryptoJS.js",
                "*.js"
            ]))
            .pipe($.uglify())
            .pipe($.concat('vendor.js'));
    }
           

    proc = proc.pipe(gulp.dest(paths.dep + "/scripts/"))
        .pipe(jsFilter.restore)
        .pipe(cssFilter);

    if (optimize == true)
        proc = proc.pipe($.concat('vendor.css'))
            .pipe($.cleanCss({ compatibility: 'ie8' }));

    return proc.pipe(gulp.dest(paths.dep + "/scripts/"))
        .pipe(cssFilter.restore)
        .pipe(fontsFilter)
        .pipe(gulp.dest(paths.dep + "/fonts/"))
        .pipe(fontsFilter.restore)
        .pipe(gulp.dest(paths.dep + "/scripts/"));

});


//gulp.task('cssLib', function(){
//
//    return gulp.src([paths.dev + '/lib/**/*.*', '!' + paths.dev + '/lib/**/*.js'])
//        .pipe($.size())
//        .pipe(gulp.dest(paths.dep + "/scripts/"));
//});

gulp.task('html', function () {
    return gulp.src([path.join(paths.dev, '/modules/**/*.html')])
        .pipe(gulp.dest(paths.dep + "/modules/"));
});

function inject(depArray, done) {
    if (!depArray) {
        depArray = ["styles", "scripts", "scriptsLib", "html", "images"];
    }

    gulp.task('inject', depArray, function () {

        var injectStyles = gulp.src([
            paths.dep + '/content/styles/*.css'
        ], { read: false }).pipe($.order([
            "typography.css",
            "*.css"
        ]));

        var injectScripts = optimize == true ? gulp.src(paths.dep + '/modules/' + bundledModFileAppData) : gulp.src([
            paths.dep + '/modules/**/*.js',
            '!' + paths.dep + '/modules/*.js',
            '!' + paths.dep + '/modules/*/*.js',
            '!' + paths.dep + '/modules/*/providers/*.js',
            '!' + paths.dep + '/modules/*/config/*.js',
            '!' + paths.dep + '/modules/core/impl/lookup.helper.js'
        ]);

        var injectStyles_bc = gulp.src([
            paths.dep + '/scripts/*.css'
        ], { read: false });

        var injectScripts_bc = gulp.src([
            paths.dep + '/scripts/*.js'
        ]).pipe($.order([
            "jquery.js",
            "angular.js",
            "angular-translate.js",
            "inputmask.js",
            "jquery.inputmask.js",
            "moment.js",
            "vendor.js",
            "metronic.js",
            "bootstrap-hover-dropdown.js",
            "aes.js",
            "angularjs-crypto.js",
            "CryptoJS.js",
            "*.js"
        ]));


        var injectOptions = {
            //ignorePath: [paths.src, paths.tmp + '/serve'],
            addRootSlash: false,
            relative: false,
            transform: function (filepath, f, p, t) {
                
                if (filepath.endsWith(".js")) {
                    return "<script type='text/javascript' src='" + filepath.replace(paths.dep + '/', '/') + "'></script>";
                }
                if (filepath.endsWith(".css")) {
                    return "<link rel='stylesheet' href='" + filepath.replace(paths.dep + '/', '/') + "' />";
                }
            }
        };

        var injectOptions_bc = {
            starttag: '<!-- inject:head:{{ext}} -->',
            addRootSlash: false,
            relative: false,
            transform: function (filepath, f, p, t) {
                if (filepath.endsWith(".js")) {
                    return "<script type='text/javascript' src='" + filepath.replace(paths.dep + '/', '/') + "'></script>";
                }
                if (filepath.endsWith(".css")) {
                    return "<link rel='stylesheet' href='" + filepath.replace(paths.dep + '/', '/') + "' />";
                }
            }
        };

        var mockarray = gulp.src([
            paths.dep + '/modules/app.module.js'
        ]);

        var injectOptions_tokenstring = {
            starttag: '<!-- inject:token -->',
            addRootSlash: false,
            relative: false,
            transform: function (filepath, f, p, t) {
                return "<script type='text/javascript'>window.appTokenName = '" + tokenName + "';</script>";
            }
        };

        return gulp.src('index.html')
            .pipe($.inject(injectStyles, injectOptions))
            .pipe($.inject(injectScripts, injectOptions))
            .pipe($.inject(injectStyles_bc, injectOptions_bc))
            .pipe($.inject(injectScripts_bc, injectOptions_bc))
            .pipe($.inject(mockarray, injectOptions_tokenstring))
            .pipe(gulp.dest(paths.dep + '/'));
    });

    if (done)
        gulp.start('inject', done);
    else
        gulp.start('inject');
}

gulp.task('clean', function (done) {
    return $.del([paths.dep + '/', paths.tmp + '/'], done);
});

gulp.task('watch', function () {
    var jsWatchPath = paths.dev + '/**/*.js';
    var htmlWatchPath = paths.dev + '/**/*.html';
    var scriptLibWatchPath = paths.dev + '/lib/**/*.*';
    var translationsWatchPath = paths.dev + '/modules/**/il8n/*.json';
    var imageWatchPath = paths.dev + '/content/images/**/*';
    var styleWatchPath = paths.dev + '/modules/**/*.scss';

    //Image file watch
    watch(styleWatchPath, batch(function (events, done) {
        console.log('Style files modified...');
        inject(["styles"], done);
    }));

    //Image file watch
    watch(imageWatchPath, batch(function (events, done) {
        console.log('Image files modified...');
        inject(["images"], done);
    }));

    //Translations file watch
    watch(translationsWatchPath, batch(function (events, done) {
        console.log('Translation files modified...');
        inject(["translations"], done);
    }));

    //JS file watch
    watch(jsWatchPath, batch(function (events, done) {
        console.log('JS files modified...');
        inject(["scripts"], done);
    }));

    //Lib file watch
    watch(scriptLibWatchPath, batch(function (events, done) {
        console.log('Lib files modified...');
        inject(["scriptsLib"], done);
    }));

    //HTML file watch
    watch(htmlWatchPath, batch(function (events, done) {
        console.log('HTML files modified...');
        gulp.start('html', done);
    }));
});

gulp.task('default', ["clean"], function () {
    inject();
});

gulp.task('test', function () {
    optimize = true;
    paths.dep = "test";
    tokenName = authTokenName.test;
    gulp.start('default');
});

gulp.task('production', function () {
    optimize = true;
    paths.dep = "prod";
    tokenName = authTokenName.production;
    gulp.start('default');
});

