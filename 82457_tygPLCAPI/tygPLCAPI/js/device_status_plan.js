
function setColor(target , color_name)
{
     $(document).ready(function() {
            $(target).toggleClass(color_name);
        });
}

function setPosition(target , top , left)
{
    $(document).ready(function() {
        $(target).css({"top": top+"px", "left": left+"px"});
    });
}