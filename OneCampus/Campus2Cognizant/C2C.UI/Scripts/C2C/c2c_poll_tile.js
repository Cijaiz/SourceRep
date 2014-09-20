$(document).ready(function () {
    c2c.poll.polltile();
});


c2c.poll.vote = function () {
    var hasPermission = $("#pollPermission").val();
    if ($("input[name='poll-vote']:checked").length > 0) {
        // one ore more checkboxes are checked
        if (hasPermission == "true" || hasPermission == "True") {

            var pollId = $("#pollId").val();
            var ansId = $("input[name='poll-vote']:checked").val();

            $("#poll-submit").attr("disabled", true);

            $.ajax({
                cache: false,
                url: c2c.poll.voteUrl,
                data: { pollId: pollId, ansId: ansId },
                success: function (data, status) {
                    if (data.status == "Success") {
                        c2c.poll.drawchart(data);
                    }
                },
                error: function (xhr, desc, err) {
                    alert('You dont have permission to vote');
                }
            });

            $("#poll-submit").attr("disabled", false);
        }
        else {
            alert('You dont have permission to vote');
        }
    }
    else {
        // no checkboxes are checked
        alert("Please select any 1 option");
    }
}

c2c.poll.polltile = function () {
    var Values;
    var Instances = new Array();
    var flag = false;
    var mylegend = { show: false };
    $.ajax({
        cache: false,
        url: c2c.poll.chartUrl,
        success: function (data) {
            if (data.result == null || data.result.PollAnswers == null || data.result.PollAnswers.length <= 0) {
                $(".left-poll").html('');
                $(".left-poll").append('<h2>NO ACTIVE POLL</h2>');
            }
            else {
                if (data.result.IsVoted) {
                    c2c.poll.drawchart(data);
                }
                else {
                    var answers = new Array(data.result.PollAnswers.length);

                    $.each(data.result.PollAnswers, function (i, ans) {
                        answers[i] = '<li><input type="radio" name="poll-vote" value="' + ans.Id + '"><label>' + ans.Answer + '</label></li>'
                    });

                    var quecontent = '<h2>POLL</h2><img src="/content/themes/base/images/poll-hand.png" class="poll-hand"><p>' + data.result.Question + '</p><input type="hidden" id="pollPermission" value="' + data.result.HasPermission + '" /><input type="hidden" id="pollId" value="' + data.result.Id + '" />'
                    var formcontent = '<form action="#" class="poll-radio"><ul id="poll-ul"></ul></form>'
                    var button = '<li><a href="#" id="poll-submit" class="submit-poll" onclick="c2c.poll.vote();">Vote Now</a></li>'

                    $(".left-poll").html('');
                    $(".right-poll").html('');

                    $(".left-poll").append(quecontent);
                    $(".right-poll").append(formcontent);
                    $.each(answers, function (i, anscontent) {
                        $("#poll-ul").append(anscontent);
                    });
                    $("#poll-ul").append(button);
                }
            }
        }
    }); //ajax
}

c2c.poll.drawchart = function (data) {
        var parentlist = data.result.PollAnswers;
        Values = new Array(parentlist.length);

        $.each(parentlist, function (i, childlist) {
            Values[i] = new Array(2);
            Values[i][0] = childlist.Answer;
            Values[i][1] = childlist.VoteCount;

            // Instances.push({ label: Values[i][0].toString() });

        }); //parentlist
        var plot1 = jQuery.jqplot('chart', [Values],
                                                    {
                                                        seriesColors: ["#f7c061", "#d75353", "#79c69c", "#8c7cc0", "#55bdc9", "#4b5de4", "#ff5800", "#0085cc"],
                                                        seriesDefaults: {
                                                            // Make this a pie chart.
                                                            renderer: jQuery.jqplot.PieRenderer,
                                                            rendererOptions: {
                                                                // Put data labels on the pie slices.
                                                                // By default, labels show the percentage of the slice.
                                                                showDataLabels: true,
                                                                dataLabelPositionFactor: 1.2,
                                                                dataLabelFormatString: '%.1f',
                                                                shadowOffset: 0,
                                                            },
                                                        },
                                                        grid: {
                                                            background: '#1e55a6',      // CSS color spec for background color of grid.
                                                            borderColor: '#1e55a6',
                                                            drawBorder: false,
                                                            shadow: false,
                                                            // CSS color spec for border around grid.     
                                                        },

                                                        legend: {
                                                            show: true, location: 'w',
                                                            background: 'transparent',
                                                            border: 'transparent',
                                                            textcolor: '#ffffff'
                                                        },

                                                        title: {  // title for the plot,
                                                            text: 'Thanks For Voting!', // title for the pl ot,
                                                            show: true,
                                                            top: 40,
                                                            left: 45,
                                                            fontSize: 18,
                                                            textColor: 'rgb(102, 102, 102)',

                                                        },

                                                    });
        $("#tile4").liveTile("play", 0);
    }
