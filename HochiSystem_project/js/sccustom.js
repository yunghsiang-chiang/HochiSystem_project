$(function () {
    $(".panel-heading").click(function (e) {
        $(this).find("span.fa-chevron-down").toggleClass("fa-chevron-up");
        $(this).find("span.fa-chevron-up").toggleClass("fa-chevron-down");
    });

    $(".solutionList li").click(function () {
        $(".solutionList .linkactive").removeClass('linkactive');
        $(this).addClass('linkactive');
    });

 
});
