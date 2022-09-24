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
      $(".blurbdiv").each(function(i, el) {
        var el = $(el);
        if (el.visible(true)) {
          el.addClass("slide"); 
        } 
      });
    });

    var element = document.getElementsByTagName("title")[0];
    var title = element.textContent;

    if (title == 'Nicholas R Johnson - Home')
    {
        $("#homeIdTop").addClass("active"); 
        $("#aboutIdTop").removeClass("active"); 
        $("#projectsIdTop").removeClass("active"); 
        $("#contactIdTop").removeClass("active"); 
        $("#innerIdTop").removeClass("active");
    }

    if (title == 'Nicholas R Johnson - About')
    {
        $("#homeIdTop").removeClass("active"); 
        $("#aboutIdTop").addClass("active"); 
        $("#projectsIdTop").removeClass("active"); 
        $("#contactIdTop").removeClass("active"); 
        $("#innerIdTop").removeClass("active");
    }

    if (title == 'Nicholas R Johnson - Contact')
    {
        $("#homeIdTop").removeClass("active"); 
        $("#aboutIdTop").removeClass("active"); 
        $("#projectsIdTop").removeClass("active"); 
        $("#contactIdTop").addClass("active"); 
        $("#innerIdTop").removeClass("active");
    }

    if (title == 'Nicholas R Johnson - Projects')
    {
        $("#homeIdTop").removeClass("active"); 
        $("#aboutIdTop").removeClass("active"); 
        $("#projectsIdTop").addClass("active"); 
        $("#contactIdTop").removeClass("active"); 
        $("#innerIdTop").removeClass("active");
    }

    if (title == 'Nicholas R Johnson - Welcome In')
    {
        $("#homeIdTop").click(function() {
            $("#homeIdTop").addClass("active");   
            $("#aboutIdTop").removeClass("active"); 
            $("#projectsIdTop").removeClass("active");  
            $("#contactIdTop").removeClass("active");  
            $("#adminIdTop").removeClass("active"); 
            $("#innerIdTop").removeClass("active");
        });
        $("#aboutIdTop").click(function() {
            $("#homeIdTop").removeClass("active");
            $("#aboutIdTop").addClass("active"); 
            $("#projectsIdTop").removeClass("active");   
            $("#contactIdTop").removeClass("active");   
            $("#adminIdTop").removeClass("active");
            $("#innerIdTop").removeClass("active"); 
        });
        $("#projectsIdTop").click(function() {
            $("#homeIdTop").removeClass("active");
            $("#aboutIdTop").removeClass("active"); 
            $("#projectsIdTop").addClass("active");   
            $("#contactIdTop").removeClass("active");   
            $("#adminIdTop").removeClass("active");
            $("#innerIdTop").removeClass("active"); 
        });
        $("#contactIdTop").click(function() {
            $("#homeIdTop").removeClass("active");
            $("#aboutIdTop").removeClass("active"); 
            $("#projectsIdTop").removeClass("active");   
            $("#contactIdTop").addClass("active");   
            $("#adminIdTop").removeClass("active");
            $("#innerIdTop").removeClass("active"); 
        });
        $("#innerIdTop").click(function() {
            $("#homeIdTop").removeClass("active");
            $("#aboutIdTop").removeClass("active"); 
            $("#projectsIdTop").removeClass("active");   
            $("#contactIdTop").removeClass("active");   
            $("#adminIdTop").removeClass("active");
            $("#innerIdTop").addClass("active"); 
        });
        $("#adminIdTop").click(function() {
            $("#homeIdTop").removeClass("active");
            $("#aboutIdTop").removeClass("active"); 
            $("#projectsIdTop").removeClass("active");   
            $("#contactIdTop").removeClass("active");   
            $("#adminIdTop").addClass("active");
            $("#innerIdTop").removeClass("active"); 
        });
    }

    if (title == 'Nicholas R Johnson - Admin')
    {
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
        $("#settingsMenu").live( "click", "#adminPersonalData", function() {
            $('#adminProfile').removeClass('active');
            $('#adminChangeEmail').removeClass('active');
            $('#adminChangePassword').removeClass('active');
            $('#adminExternalLogins').removeClass('active');
            $('#adminTwoFactorAuthentication').removeClass('active');
            $('#adminPersonalData').addClass('active');
        });
    } 
});