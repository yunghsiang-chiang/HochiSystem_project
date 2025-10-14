$("#section2 .item").hover(
  function () {
    $(this).find('.hover').animate({ opacity: 0.2 }, 200);
    $(this).find('.detail').show(200);
    $(this).find('.detail').css('display', 'flex');

  },
  function () {
    $(this).find('.hover').animate({ opacity: 1 }, 200);
    $(this).find('.detail').hide(200);
  }
);

$("#section3 .item").hover(
  function () {
    $(this).find('.hover').animate({ opacity: 0.2 }, 200);
    $(this).find('.detail').show(200);
    $(this).find('.detail').css('display', 'flex');

  },
  function () {
    $(this).find('.hover').animate({ opacity: 1 }, 200);
    $(this).find('.detail').hide(200);
  }
);

$("#section6 .el-card").hover(
  function () {
    $(this).find('.backup').animate({ opacity: 0 }, 200);
    $(this).find('.backup').animate({ 'z-index': -1 }, 200);

  },
  function () {
    $(this).find('.backup').animate({ 'z-index': 1 }, 0);
    $(this).find('.backup').animate({ opacity: 0.3 }, 200);
  }
);
$(".modal-body .pop-hide").hover(
  function () {
    $(this).find('.backup').show();
    $(this).find('.info').css('color', 'white');
  },
  function () {
    $(this).find('.backup').hide();
    $(this).find('.info').css('color', 'black');
  }
);
