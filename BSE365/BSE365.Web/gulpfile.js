var gulp = require('gulp');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var watch = require('gulp-watch');
var sourcemaps = require('gulp-sourcemaps');
var less = require('gulp-less');
var livereload = require('gulp-livereload');

//bundle Javascript
var paths = {
    'Common1': [
        'bower_components/jquery/dist/jquery.min.js',
        'bower_components/bootstrap/dist/js/bootstrap.min.js',
        'bower_components/angular/angular.min.js',
        'bower_components/angular-bootstrap/ui-bootstrap-tpls.min.js',
    ],
    'Common2': [
        'bower_components/moment/min/moment-with-locales.min.js',
        'bower_components/angular-ui-router/release/angular-ui-router.min.js',
        'bower_components/angular-resource/angular-resource.min.js',
        'bower_components/angular-local-storage/dist/angular-local-storage.min.js',
        'bower_components/angular-smart-table/dist/smart-table.min.js',
        'bower_components/angular-ui-tree/dist/angular-ui-tree.min.js',
        'bower_components/angular-file-upload/dist/angular-file-upload.min.js',
        'bower_components/angular-messages/angular-messages.min.js',
        'bower_components/angular-loading-bar/build/loading-bar.min.js',
        'bower_components/angular-ui-validate/dist/validate.min.js',
        'bower_components/angular-re-captcha/angular-re-captcha.js',
        'bower_components/angular-ui-notification/dist/angular-ui-notification.min.js',
        'bower_components/angular-signalr-hub/signalr-hub.min.js',
        'bower_components/humanize-duration/humanize-duration.js',
        'bower_components/angular-timer/dist/angular-timer.min.js',
        'bower_components/underscore/underscore-min.js',
        'bower_components/ngImgCrop/compile/minified/ng-img-crop.js',
        'bower_components/angular-sanitize/angular-sanitize.min.js',
        'bower_components/signalr/jquery.signalR.min.js',
    ],
    'MainApp': [
        'app/main/*.js',
        'app/main/**/*.js',
    ],
    'CommonApp': [
        'app/common/*.js',
        'app/common/**/*.js',
    ],
    'AuthApp': [
        'app/authentication/*.js',
        'app/authentication/**/*.js',
    ],
    'images': "Content/images/",
    'fonts': "fonts/"
};

gulp.task('CommonJS1', function () {
    return gulp.src(paths['Common1'])
        .pipe(concat('Common1.js'))
        .pipe(uglify())
        .pipe(gulp.dest('Scripts'));
});

gulp.task('CommonJS2', function () {
    return gulp.src(paths['Common2'])
        .pipe(concat('Common2.js'))
        .pipe(uglify())
        .pipe(gulp.dest('Scripts'));
});

gulp.task('MainAppJS', function () {
    return gulp.src(paths['MainApp'])
        .pipe(concat('MainApp.js'))
        .pipe(uglify())
        .pipe(gulp.dest('Scripts'));
});

gulp.task('CommonAppJS', function () {
    return gulp.src(paths['CommonApp'])
        .pipe(concat('CommonApp.js'))
        .pipe(uglify())
        .pipe(gulp.dest('Scripts'));
});

gulp.task('AuthAppJS', function () {
    return gulp.src(paths['AuthApp'])
        .pipe(concat('AuthApp.js'))
        .pipe(uglify())
        .pipe(gulp.dest('Scripts'));
});

gulp.task('js', ['CommonJS1', 'CommonJS2', 'MainAppJS', 'CommonAppJS', 'AuthAppJS'], function () {
    //nothing to do here
});

//bundle Css
gulp.task('less', function () {
    return gulp.src([
            'bower_components/bootstrap/less/bootstrap.less',
            'bower_components/angular-ui-tree/dist/angular-ui-tree.css',
            'bower_components/ngImgCrop/compile/unminified/ng-img-crop.css',
            'bower_components/angular-loading-bar/build/loading-bar.css',
            'bower_components/angular-ui-notification/dist/angular-ui-notification.css',
            'content/main.less',
    ])
        .pipe(sourcemaps.init())
        .pipe(less())
        .pipe(sourcemaps.write('./', { includeContent: false, sourceRoot: 'Content' }))
        .pipe(gulp.dest('Content'));
});

//gulp.task("images", function () {
//    return gulp.src([paths['images'] + "**/*.*", paths['images'] + "**/*"])
//        .pipe(gulp.dest(paths.dist + "images/"));
//});

//gulp.task("fonts", function () {
//    return gulp.src(paths['fonts'] + "*")
//        .pipe(gulp.dest(paths.dist + "fonts/"));
//});

//Watch change
gulp.task('watch', ['less', 'js'], function () {
    gulp.watch('Content/**/*.less', ['less']);
    gulp.watch('app/**/*.js', ['js']);
    gulp.watch('app/**/**/*.js', ['js']);
    //gulp.watch('Content/images/*.jpg', ['jpg']);
    //gulp.watch('Content/images/*.png', ['png']);
    //gulp.watch('fonts/*.ttf', ['ttf']);
});

var LiveReloadLessPath = [
    'Content/*.less',
    'Content/**/*.less'
];

var LiveReloadJSPath = [
    paths['CommonJS1'],
    paths['CommonJS2'],
    paths['MainAppJS'],
    paths['CommonAppJS'],
    paths['AuthAppJS'],
    'Gulpfile.js'
];

var LiveReloadHTMLPath = [
    'app/**/**/*.html'
];

//liverload
gulp.task('livereload', function () {
    livereload.listen();
    gulp.watch(paths['Common1'], ['CommonJS1'])
        .on('change', livereload.changed);
    gulp.watch(paths['Common2'], ['CommonJS2'])
        .on('change', livereload.changed);
    gulp.watch(LiveReloadHTMLPath)
        .on('change', livereload.changed);
    gulp.watch(LiveReloadLessPath, ['less'])
        .on('change', livereload.changed);
});