(function($){
    var $w=$(window);
    $.fn.visible = function(partial,hidden,direction,container){

        if (this.length < 1)
            return;
	
	direction = direction || 'both';
	    
        var $t          = this.length > 1 ? this.eq(0) : this,
						isContained = typeof container !== 'undefined' && container !== null,
						$c				  = isContained ? $(container) : $w,
						wPosition        = isContained ? $c.position() : 0,
            t           = $t.get(0),
            vpWidth     = $c.outerWidth(),
            vpHeight    = $c.outerHeight(),
            clientSize  = hidden === true ? t.offsetWidth * t.offsetHeight : true;

        if (typeof t.getBoundingClientRect === 'function'){

            var rec = t.getBoundingClientRect(),
                tViz = isContained ?
												rec.top - wPosition.top >= 0 && rec.top < vpHeight + wPosition.top :
												rec.top >= 0 && rec.top < vpHeight,
                bViz = isContained ?
												rec.bottom - wPosition.top > 0 && rec.bottom <= vpHeight + wPosition.top :
												rec.bottom > 0 && rec.bottom <= vpHeight,
                lViz = isContained ?
												rec.left - wPosition.left >= 0 && rec.left < vpWidth + wPosition.left :
												rec.left >= 0 && rec.left <  vpWidth,
                rViz = isContained ?
												rec.right - wPosition.left > 0  && rec.right < vpWidth + wPosition.left  :
												rec.right > 0 && rec.right <= vpWidth,
                vVisible   = partial ? tViz || bViz : tViz && bViz,
                hVisible   = partial ? lViz || rViz : lViz && rViz,
		vVisible = (rec.top < 0 && rec.bottom > vpHeight) ? true : vVisible,
                hVisible = (rec.left < 0 && rec.right > vpWidth) ? true : hVisible;

            if(direction === 'both')
                return clientSize && vVisible && hVisible;
            else if(direction === 'vertical')
                return clientSize && vVisible;
            else if(direction === 'horizontal')
                return clientSize && hVisible;
        } else {

            var viewTop 				= isContained ? 0 : wPosition,
                viewBottom      = viewTop + vpHeight,
                viewLeft        = $c.scrollLeft(),
                viewRight       = viewLeft + vpWidth,
                position          = $t.position(),
                _top            = position.top,
                _bottom         = _top + $t.height(),
                _left           = position.left,
                _right          = _left + $t.width(),
                compareTop      = partial === true ? _bottom : _top,
                compareBottom   = partial === true ? _top : _bottom,
                compareLeft     = partial === true ? _right : _left,
                compareRight    = partial === true ? _left : _right;

            if(direction === 'both')
                return !!clientSize && ((compareBottom <= viewBottom) && (compareTop >= viewTop)) && ((compareRight <= viewRight) && (compareLeft >= viewLeft));
            else if(direction === 'vertical')
                return !!clientSize && ((compareBottom <= viewBottom) && (compareTop >= viewTop));
            else if(direction === 'horizontal')
                return !!clientSize && ((compareRight <= viewRight) && (compareLeft >= viewLeft));
        }
    };
  $.fn.visible = function(partial) {
    
    var $t            = $(this),
        $w            = $(window),
        viewTop       = $w.scrollTop(),
        viewBottom    = viewTop + $w.height(),
        _top          = $t.offset().top,
        _bottom       = _top + $t.height(),
        compareTop    = partial === true ? _bottom : _top,
        compareBottom = partial === true ? _top : _bottom;
  
    return ((compareBottom <= viewBottom) && (compareTop >= viewTop));

  };

})(jQuery);    


$(document).ready(function() {   
    $(window).scroll(function(event) {
      $(".blurbdiv ").each(function(i, el) {
        var el = $(el);
        if (el.visible(true)) {
          el.addClass("slide"); 
        } 
      });
    });

    if (title == 'Nicholas R Johnson - Home')
    {
        $("#homeIdTop").css("color", "#efad32"); 
        $("#aboutIdTop").css("color", "#b27c17"); 
        $("#projectsIdTop").css("color", "#b27c17"); 
        $("#contactIdTop").css("color", "#b27c17"); 
        $("#innerIdTop").css("color", "#b27c17");
    }

    if (title == 'Nicholas R Johnson - About')
    {
        $("#homeIdTop").css("color", "#b27c17"); 
        $("#aboutIdTop").css("color", "#efad32"); 
        $("#projectsIdTop").css("color", "#b27c17"); 
        $("#contactIdTop").css("color", "#b27c17"); 
        $("#innerIdTop").css("color", "#b27c17");
    }

    if (title == 'Nicholas R Johnson - Contact')
    {
        $("#homeIdTop").css("color", "#b27c17"); 
        $("#aboutIdTop").css("color", "#b27c17"); 
        $("#projectsIdTop").css("color", "#b27c17"); 
        $("#contactIdTop").css("color", "#efad32"); 
        $("#innerIdTop").css("color", "#b27c17");
    }

    if (title == 'Nicholas R Johnson - Projects')
    {
        $("#homeIdTop").css("color", "#b27c17"); 
        $("#aboutIdTop").css("color", "#b27c17"); 
        $("#projectsIdTop").css("color", "#efad32"); 
        $("#contactIdTop").css("color", "#b27c17"); 
        $("#innerIdTop").css("color", "#b27c17");
    }

    if (title == 'Nicholas R Johnson - Welcome In')
    {
        $("#homeIdTop").click(function() {
            $("#homeIdTop").css("color", "#efad32"); 
            $("#aboutIdTop").css("color", "#b27c17"); 
            $("#projectsIdTop").css("color", "#b27c17"); 
            $("#contactIdTop").css("color", "#b27c17"); 
            $("#insideIdTop").css("color", "#b27c17");
        });
        $("#aboutIdTop").click(function() {
            $("#homeIdTop").css("color", "#b27c17"); 
            $("#aboutIdTop").css("color", "#efad32"); 
            $("#projectsIdTop").css("color", "#b27c17"); 
            $("#contactIdTop").css("color", "#b27c17"); 
            $("#insideIdTop").css("color", "#b27c17");
        });
        $("#projectsIdTop").click(function() {
            $("#homeIdTop").css("color", "#b27c17"); 
            $("#aboutIdTop").css("color", "#b27c17"); 
            $("#projectsIdTop").css("color", "#efad32"); 
            $("#contactIdTop").css("color", "#b27c17"); 
            $("#insideIdTop").css("color", "#b27c17");
        });
        $("#contactIdTop").click(function() {
            $("#homeIdTop").css("color", "#b27c17"); 
            $("#aboutIdTop").css("color", "#b27c17"); 
            $("#projectsIdTop").css("color", "#b27c17"); 
            $("#contactIdTop").css("color", "#efad32"); 
            $("#insideIdTop").css("color", "#b27c17");
        });
        $("#insideIdTop").click(function() {
            $("#homeIdTop").css("color", "#b27c17"); 
            $("#aboutIdTop").css("color", "#b27c17"); 
            $("#projectsIdTop").css("color", "#b27c17"); 
            $("#contactIdTop").css("color", "#b27c17"); 
            $("#insideIdTop").css("color", "#efad32");
        });
    }

    if (title == 'Nicholas R Johnson - Admin')
    {
        $(document).ready( function() {
            $("#settingsMenu").live( "click", "#adminProfile", function() {
                $('#adminProfile').addClass('active');
                $('#adminChangeEmail').removeClass('active');
                $('#adminChangePassword').removeClass('active');
                $('#adminExternalLogins').removeClass('active');
                $('#adminTwoFactorAuthentication').removeClass('active');
                $('#adminPersonalData').removeClass('active');
            });

            $("#settingsMenu").live( "click", "#adminChangeEmail", function() {
                $('#adminProfile').removeClass('active');
                $('#adminChangeEmail').addClass('active');
                $('#adminChangePassword').removeClass('active');
                $('#adminExternalLogins').removeClass('active');
                $('#adminTwoFactorAuthentication').removeClass('active');
                $('#adminPersonalData').removeClass('active');
            });
            $("#settingsMenu").live( "click", "#adminChangePassword", function() {
                $('#adminProfile').removeClass('active');
                $('#adminChangeEmail').removeClass('active');
                $('#adminChangePassword').addClass('active');
                $('#adminExternalLogins').removeClass('active');
                $('#adminTwoFactorAuthentication').removeClass('active');
                $('#adminPersonalData').removeClass('active');
            });
            $("#settingsMenu").live( "click", "#adminExternalLogins", function () {
                $('#adminProfile').removeClass('active');
                $('#adminChangeEmail').removeClass('active');
                $('#adminChangePassword').removeClass('active');
                $('#adminExternalLogins').addClass('active');
                $('#adminTwoFactorAuthentication').removeClass('active');
                $('#adminPersonalData').removeClass('active');
            });
            $("#settingsMenu").live( "click", "#adminTwoFactorAuthentication", function() {
                $('#adminProfile').removeClass('active');
                $('#adminChangeEmail').removeClass('active');
                $('#adminChangePassword').removeClass('active');
                $('#adminExternalLogins').removeClass('active');
                $('#adminTwoFactorAuthentication').addClass('active');
                $('#adminPersonalData').removeClass('active');
            });
            $("#settingsMenu").live( "click", "@adminPersonalData", function() {
                $('#adminProfile').removeClass('active');
                $('#adminChangeEmail').removeClass('active');
                $('#adminChangePassword').removeClass('active');
                $('#adminExternalLogins').removeClass('active');
                $('#adminTwoFactorAuthentication').removeClass('active');
                $('#adminPersonalData').addClass('active');
            });
        });
    } 
});