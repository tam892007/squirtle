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
        'bower_components/angular-bootstrap/ui-bootstrap-tpls.js'
    ],
    'MainApp': [
        'app/main/*.js',
        'app/main/**/*.js',
    ]
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


gulp.task('js', ['CommonJS', 'MainAppJS'], function () {
    //nothing to do here
});

//bundle Css
gulp.task('less', function () {
    return gulp.src([
            'bower_components/bootstrap/less/bootstrap.less',
            'content/main.less',
    ])
        .pipe(sourcemaps.init())
        .pipe(less())
        .pipe(sourcemaps.write('./', { includeContent: false, sourceRoot: 'Content' }))
        .pipe(gulp.dest('Content'));
});

//Watch change
gulp.task('watch', ['less', 'js'], function () {
    gulp.watch('Content/**/*.less', ['less']);
    gulp.watch('app/**/*.js', ['js']);
    gulp.watch('app/**/**/*.js', ['js']);
});

var LiveReloadLessPath = [
    'Content/*.less',
    'Content/**/*.less'
];

var LiveReloadJSPath = [
    paths['CommonJS'],
    paths['MainAppJS'],
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