var gulp = require('gulp');
var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var watch = require('gulp-watch');
var sourcemaps = require('gulp-sourcemaps');
var less = require('gulp-less');
var livereload = require('gulp-livereload');

//bundle Javascript
var paths = {
    'Common': [
        'bower_components/jquery/dist/jquery.js',
        'bower_components/bootstrap/dist/js/bootstrap.js',     
        'bower_components/angular/angular.js',
        'bower_components/ui-router/release/angular-ui-router.js',
        'bower_components/angular-bootstrap/ui-bootstrap-tpls.js',
        'bower_components/angular-resource/angular-resource.js',
        'bower_components/angular-local-storage/dist/angular-local-storage.js',
        'bower_components/angular-smart-table/dist/smart-table.js',
        'bower_components/angular-ui-tree/dist/angular-ui-tree.js',
        'bower_components/underscore/underscore.js',
        'bower_components/angular-file-upload/dist/angular-file-upload.js',
        'bower_components/ngImgCrop/compile/unminified/ng-img-crop.js',
        'bower_components/angular-messages/angular-messages.js',
        'bower_components/angular-loading-bar/build/loading-bar.js',
        'bower_components/angular-ui-validate/dist/validate.js',
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

gulp.task('CommonJS', function () {
    return gulp.src(paths['Common'])
        .pipe(concat('Common.js'))
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

gulp.task('js', ['CommonJS', 'MainAppJS', 'CommonAppJS', 'AuthAppJS'], function () {
    //nothing to do here
});

//bundle Css
gulp.task('less', function () {
    return gulp.src([
            'bower_components/bootstrap/less/bootstrap.less',
            'bower_components/angular-ui-tree/dist/angular-ui-tree.css',
            'bower_components/ngImgCrop/compile/unminified/ng-img-crop.css',
            'bower_components/angular-loading-bar/build/loading-bar.css',
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
    paths['CommonJS'],
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
    gulp.watch(paths['Common'], ['CommonJS'])
        .on('change', livereload.changed);
    gulp.watch(LiveReloadHTMLPath)
        .on('change', livereload.changed);
    gulp.watch(LiveReloadLessPath, ['less'])
        .on('change', livereload.changed);
});