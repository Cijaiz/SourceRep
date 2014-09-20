//test script FLIP EFFECTS -->

		//play on hover
			var $tiles = $(".BlogtilePost").liveTile({ 
			    playOnHover:true,
			    repeatCount: 0,
			    delay: 0,
			    startNow:false 
			});

			function article(id) {
			    window.location.href = "/Blog/Article/" + id;
			}