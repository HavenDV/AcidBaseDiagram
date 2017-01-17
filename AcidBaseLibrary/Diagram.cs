using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

/*

//***** Graph Scale
	var scale = "8";														// Designed for 10. Tested between 4 and 15.

//***** Regional Units
	var Region = "US";
	if (Region == "US")	{
		var metUnits = "mEq/L";											// Metabolic Units
		var respUnits = "mmHg";											// Respiratory Units
	}
	else	{
		var metUnits = "mmol/L";											// Metabolic Units
		var respUnits = "kPa";											// Respiratory Units
	}
	
//***** Font Style
	var graphFont = "normal " + scale * 2.5 + "px arial";

// ***** Graph Colors
	var graphBackground = "#000000";									// Black Background
	var graphBorder = "#FF0000";										// Cyan Border
	var graphBackgroundBorder = "#000000";							// Shows Whole Windmill if lighter
	var NeutralText = "#333333";										// Neutral Areas
	var AcidText = "#FF0000";										// Neutral Areas
	var AlkaliText = "#0000FF";										// Neutral Areas
	var AcidColor = "#990000";											// Acid Areas Red
	var AcidBorderColor = "#990000";									// Acid Areas Red
	var AlkaliColor = "#000099";										// Alkaline Areas Blue
	var AlkaliBorderColor = "#000099";									// Alkaline Areas Blue
	var graphOtherZone = "#333333";									// Other Zones Grey
	var BEisopleths = "#33FF33";										// BE Color
	var PCO2isopleths = "#FF0000";										// PCO2 Color
	var pHisopleths = "#FFFF00";										// pH Color
	var BICisopleths = "#77BBFF";										// bic color
	var color1;															// Temporary Color
	var color2;															// Temporary Color

	var divs = document.getElementsByClassName('Value');
	for (var i=0; i<4; i++ ) {									// Go through all TDs
		divs[i].style.backgroundColor = "#000000";						// Background Color
		divs[i].style.font = graphFont;									// Font details
		divs[i].style.height = scale*9 + 'px';
		divs[i].style.width = scale*10 + 'px';
		divs[i].style.marginTop = 2*scale + "px";
		divs[i].style.marginBottom = 2*scale + "px";
	}

	divs[0].style.color = pHisopleths;
	divs[1].style.color = PCO2isopleths;
	divs[2].style.color = BEisopleths;
	divs[3].style.color = BICisopleths;

	var spaces = document.getElementsByClassName('Space');
	for (var i=0; i<3; i++ ) {									// Go through all Spaces
//		spaces[i].style.marginLeft = (1.9 * scale - 6 + "px");						// Space
//		spaces[i].style.marginLeft = "13px";						// Space
	}
	
	document.getElementById('Report').style.font = graphFont;
	document.getElementById('Report').style.width = (98 * scale +17  +"px");
	document.getElementById('Report').style.height = (7 * scale +"px");
	document.getElementById('Report').style.padding = (scale +"px");
//	document.getElementById('Values').style.borderSpacing = (scale*1.7-7 + 'px 0px');

	document.getElementById('Respiratory').style.font = graphFont;
	document.getElementById('Respiratory').style.width = (88 * scale -4 +"px");

	document.getElementById('Metabolic').style.font = graphFont;
	document.getElementById('Metabolic').style.width = (7 * scale+"px");
	document.getElementById('Metabolic').style.top = (15 * scale+"px"); 
	document.getElementById('Metabolic').style.left = (30-3.5*scale+"px"); 

// ***** Clinical Limits and Ranges and Colors - for SBE/PCO2 Diagram
	var PCO2Low = 10;														// Resp Min
	var PCO2High = 100;													// Resp Max
	var PCO2Range = PCO2High-PCO2Low;									// Ratio x-axis 

	var BELow = -30;														// Met Min
	var BEHigh = 30;														// Met Max
	var BERange = BEHigh - BELow;										// Ratio y-axis
	
	var pHSlope = 0.56871952;											// Slope of pH 7.4
	var respWidth = 5;													// Width Typical Zone
	var metWidth = respWidth * pHSlope;								// Height Typical Zone
	var normWidth = 3.0;													// Width Normal Area
	var normHeight = normWidth*pHSlope;								// Height Normal Area
	var respSlope = 0.4;													// Chr Res Zone Slope
	var metAcSlope = 1.0;												// Met Ac Zone Slope
	var metAlkSlope = 1/0.6;											// Met Alk Zone Slope
	var respMagn;															// Magnitude of Respiratory Change
	var metMagn;															// Magnitude of Metabolic Change

//***** Graph Dimensions
	var canvas;
	var context;

	var xRange = PCO2Range * scale;									// X-Axis length
	var yRange = BERange * scale;										// Y-Axis length
	
	var xPlotOrigin = 0;													// Left edge of Graph
	var yPlotOrigin = 0;													// Top edge of Graph

	var xLow = - scale * PCO2Low + 1;									// Left Edge for graph
	var xHigh = xLow + xRange;											// Right edge

	var yHigh = - scale * BEHigh + 1;									// Top edge for graph
	var yLow = yHigh + yRange;											// Bottom edge

	var xTemp1, yTemp1, xTemp2, yTemp2;								// Temporary plot values
	var temp, temp1, temp2;												// Temporary variables

	var mx, my, x1, y1, x2, y2;											// Graph Plot Variables

	var main = document.getElementById("main");						// Locate "Main" Canvas
	var contextmain = main.getContext("2d");							//	Context
	
	main.width = xRange+2;												// Canvas Width
	main.height = yRange+2;												// Canvas Height


// ***** Clinical Limits and Values
	var PCO2;																// Clinical PCO2
	var pH;																// Clinical pH
	var BE;																// Clinical Base Excess
	var H;																	// Clinical [H+]
	var bic;																// Clinical Bicarb
	var K;																	// Clinical Equil K
	var pK;																// Clinical pK
	var logPCO2;															// Clinical log PCO2
	var PCO2Change;														// Change PCO2 + and -
	var deltaPCO2;														// Delta PCO2
	var deltaBE;															// Delta BE
	var pHdisp;															// Display pH
	var PCO2kPadisp;														// Display PCO2 kPa
	var PCO2disp;															// Display PCO2
	var BEdisp;															// Display BE
	var bicdisp;															// Display Bic
	var Hdisp;																// Display [H+]
	var reportStringdisp;												// Display Text Report

// ***** X-Plotting Values
	var xK = new Array();	
		xK[0] = xLow + scale * PCO2Low;													// Left edge diagram (10)
		xK[1] = xLow + scale * PCO2Low;													// Corner Met Ac (10)
		xK[2] = xLow + scale * (PCO2Low + respWidth);									// Corner Met Ac (15)
		xK[3] = xLow + scale * (40 - 2 * metWidth/respSlope);							// Bend in Ac Resp Alk (26)
		xK[4] = xLow + scale * (40-(respWidth+metWidth)/(metAcSlope-respSlope));	// Bend Met Ac (27)
		xK[5] = xLow + scale * (40 - respWidth);											// Left Pure Met Ac/Alk (35)
		xK[6] = xLow + scale * (40 - normWidth);											// Lt Normal (37)
		xK[7] = xLow + scale * (40 - 0.5 * normWidth * pHSlope / metAlkSlope);		// Normal Minus (39)
		xK[8] = xLow + scale * 40;															// Normal (40)
		xK[9] = xLow + scale * (40 + 0.5 * normWidth * pHSlope / metAlkSlope);		// Normal Plus(41)
		xK[10] = xLow + scale * (40 + normWidth);											// Rt Normal (43)
		xK[11] = xLow + scale * (40 + respWidth);											// Rt Pure Met Ac/Alk (45)
		xK[12] = xLow + scale * (40+(respWidth*metAlkSlope+metWidth)/(metAlkSlope-respSlope));	//(49)
		xK[13] = xLow + scale * (40 + BEHigh * 0.6 - respWidth);						// Met Alk Border (53)
		xK[14] = xLow + scale * (40 + 2 * metWidth /respSlope);						// Chr Resp Alk Bend (54)
		xK[15] = xLow + scale * (40 + BEHigh * 0.6 + respWidth);						// Met Alk Border (63)
		xK[16] = xLow + scale * (40 + BEHigh/pHSlope);									// Rt End pH = scale * 7.4 (92)
		xK[17] = xLow + scale * PCO2High;													// Right Edge Diagram (100)
		
// ***** Y-Plotting Values
	var yK = new Array();
		yK[0] = yLow - scale * BELow;														// Low edge diagram (-30)
		yK[1] = yLow - scale * BELow;														// Corner Met Ac (-30)
		yK[2] = yLow - scale * (BELow + respWidth);										// Edge Met Ac (-27)
		yK[3] = yLow - scale * ((PCO2Low -40) * respSlope - metWidth);				// Left End Met Ac (-15)
		yK[4] = yLow - scale * (-respWidth *metAcSlope * 2);							// Bend in Met Ac (-10)
		yK[5] = yLow - scale * ((PCO2Low -40) * respSlope + metWidth);				// Left End Met Ac (-9)
		yK[6] = yLow - scale * (((40-(respWidth+metWidth)/(metAcSlope-respSlope)) - 40) * metAcSlope + respWidth);		// Bend in Chr Resp Alk (-8)
		yK[7] = yLow - scale * (0 - metWidth);											// Low Pure Met (-3)
		yK[8] = yLow - scale * (0 - normHeight );											// Low Normal (-2)
		yK[9] = yLow - scale * (0 - 0.5 * respSlope * normWidth);						// Normal Minus (-1)
		yK[10] = yLow - scale * 0;															// Normal (0)
		yK[11] = yLow - scale * (0 + 0.5 * respSlope * normWidth);						// Normal Plus (1)
		yK[12] = yLow - scale * (0 + normHeight);											// Hi Normal (2)
		yK[13] = yLow - scale * (0 + metWidth);											// Hi Pure Res (3)
		yK[14] = yLow - scale * (((40+(respWidth*metAlkSlope+metWidth)/(metAlkSlope-respSlope)) - 40)*respSlope + metWidth);		// Bend Chr Resp Ac (6)
		yK[15] = yLow - scale * (respWidth * metAlkSlope * 2);							// Bend Pure Met Alk (16)
		yK[16] = yLow - scale * (pHSlope * (40 - PCO2Low));								// Left Neg 7.4 Slope (17)
		yK[17] = yLow - scale * ((PCO2High -40) * respSlope -metWidth);				// Rt Resp Alk Low (21)
		yK[18] = yLow - scale * ((PCO2High -40) * respSlope +metWidth);				// Rt Resp Alk High (26)
		yK[19] = yLow - scale * BEHigh;													// Top Edge Diagram (30)

// ***** Polygon Coordinates
	var polygons = [
		[ {x:xK[6], y:yK[12]}, {x:xK[10], y:yK[12]}, {x:xK[10], y:yK[8]}, {x:xK[6], y:yK[8]}, {x:xK[6], y:yK[12]} ],																	// 0.Normal
		[ {x:xK[5], y:yK[19]}, {x:xK[11], y:yK[19]}, {x:xK[11], y:yK[15]}, {x:xK[9], y:yK[12]}, {x:xK[7], y:yK[12]}, {x:xK[5], y:yK[15]}, {x:xK[5], y:yK[19]} ], 				// 1. Pure M-
		[ {x:xK[11], y:yK[19]}, {x:xK[13], y:yK[19]}, {x:xK[11], y:yK[15]}, {x:xK[11], y:yK[19]} ],																						// 2.
		[ {x:xK[13], y:yK[19]}, {x:xK[15], y:yK[19]}, {x:xK[12], y:yK[14]}, {x:xK[10], y:yK[12]}, {x:xK[9], y:yK[12]}, {x:xK[11], y:yK[15]}, {x:xK[13], y:yK[19]} ], 			// 3. Chr M-
		[ {x:xK[15], y:yK[19]}, {x:xK[17], y:yK[19]}, {x:xK[17], y:yK[18]}, {x:xK[12], y:yK[14]}, {x:xK[15], y:yK[19]} ],																// 4.
		[ {x:xK[17], y:yK[18]}, {x:xK[17], y:yK[17]}, {x:xK[14], y:yK[13]}, {x:xK[10], y:yK[11]}, {x:xK[10], y:yK[12]}, {x:xK[12], y:yK[14]}, {x:xK[17], y:yK[18]} ], 		// 5. Chr R Ac
		[ {x:xK[17], y:yK[17]}, {x:xK[17], y:yK[13]}, {x:xK[14], y:yK[13]}, {x:xK[17], y:yK[17]} ],																						// 6.
		[ {x:xK[17], y:yK[13]}, {x:xK[17], y:yK[7]}, {x:xK[14], y:yK[7]}, {x:xK[10], y:yK[9]}, {x:xK[10], y:yK[11]}, {x:xK[14], y:yK[13]}, {x:xK[17], y:yK[13]} ], 			// 7. Resp Ac
		[ {x:xK[17], y:yK[7]}, {x:xK[17], y:yK[0]}, {x:xK[16], y:yK[0]}, {x:xK[10], y:yK[8]}, {x:xK[10], y:yK[9]}, {x:xK[14], y:yK[7]}, {x:xK[17], y:yK[7]} ], 				// 8. 
		[ {x:xK[16], y:yK[0]}, {x:xK[11], y:yK[0]}, {x:xK[11], y:yK[4]}, {x:xK[9], y:yK[8]}, {x:xK[10], y:yK[8]}, {x:xK[16], y:yK[0]} ], 											// 9. 
		[ {x:xK[11], y:yK[0]}, {x:xK[5], y:yK[0]}, {x:xK[5], y:yK[4]}, {x:xK[7], y:yK[8]}, {x:xK[9], y:yK[8]}, {x:xK[11], y:yK[5]}, {x:xK[11], y:yK[0]} ], 						// 10 Pure M+ No R+
		[ {x:xK[5], y:yK[0]}, {x:xK[2], y:yK[0]}, {x:xK[5], y:yK[4]}, {x:xK[5], y:yK[0]} ],																								// 11.
		[ {x:xK[2], y:yK[0]}, {x:xK[1], y:yK[1]}, {x:xK[0], y:yK[2]}, {x:xK[4], y:yK[6]}, {x:xK[6], y:yK[8]}, {x:xK[7], y:yK[8]}, {x:xK[5], y:yK[4]}, {x:xK[2], y:yK[0]} ], 	// 12 Met + Typical
		[ {x:xK[0], y:yK[2]}, {x:xK[0], y:yK[3]}, {x:xK[4], y:yK[6]}, {x:xK[0], y:yK[2]} ],																								// 13.
		[ {x:xK[0], y:yK[3]}, {x:xK[0], y:yK[5]}, {x:xK[3], y:yK[7]}, {x:xK[6], y:yK[9]}, {x:xK[6], y:yK[8]}, {x:xK[4], y:yK[6]}, {x:xK[0], y:yK[3]} ], 						// 14 Chr R-. 
		[ {x:xK[0], y:yK[5]}, {x:xK[0], y:yK[7]}, {x:xK[3], y:yK[7]}, {x:xK[0], y:yK[5]} ],																								// 15.
		[ {x:xK[0], y:yK[7]}, {x:xK[0], y:yK[13]}, {x:xK[3], y:yK[13]}, {x:xK[6], y:yK[11]}, {x:xK[6], y:yK[9]}, {x:xK[3], y:yK[7]}, {x:xK[0], y:yK[7]} ], 						// 18 Resp Alk. 
		[ {x:xK[0], y:yK[13]}, {x:xK[0], y:yK[16]}, {x:xK[6], y:yK[12]}, {x:xK[6], y:yK[11]}, {x:xK[3], y:yK[13]}, {x:xK[0], y:yK[13]} ], 											// 17. 
		[ {x:xK[0], y:yK[16]}, {x:xK[0], y:yK[19]}, {x:xK[5], y:yK[19]}, {x:xK[5], y:yK[15]}, {x:xK[7], y:yK[12]}, {x:xK[6], y:yK[12]}, {x:xK[0], y:yK[16]} ], 				// 18. 
		[ {x:2, y:2}, {x:3, y:2}, {x:2, y:3}, {x:2, y:2} ] 																																		// 19. Avoids Visible Final Segment
	];


// ***** Report Code strings.  (18 options, 10 variables). Each line corresponds to a "zone"
// A "*" represents a value which will depend on severity.  
	var codes = new Array();
	codes[0]="0000000000";												// Normal
	codes[1]="01*221*131";												// Pure Met Alk & No Comp
	codes[2]="04*222*111";												// Met Alk > Resp Acid
	codes[3]="04*222*114";												// Met Alk & Comp Typ
	codes[4]="04*222*111";												// Met Alk > Resp Acid
	codes[5]="01*111*222";												// Resp Acid & M CompTyp
	codes[6]="01*111*221";												// Resp Acid > M Alk
	codes[7]="01*111*236";												// Pure Resp Acid No M Comp
	codes[8]="03*112*211";												// Resp Acid > Met Acid
	codes[9]="03*212*111";												// Met Acid > Resp Acid
	codes[10]="01*211*131";												// Pure Met Acid & No Comp
	codes[11]="01*211*121";												// Met Acid &  Resp Alk
	codes[12]="01*211*124";												// Met Acid &  Comp Typ
	codes[13]="04*212*121";												// Met Acid & Resp Alk
	codes[14]="04*122*215";												// Resp Alk & M CompTyp
	codes[15]="01*121*231";												// Resp Alk > M Acid
	codes[16]="01*121*236";												// Pure Resp Alk No M Comp
	codes[17]="03*122*221";												// Resp Alk > Met Alk
	codes[18]="03*222*121";												// Met Alk > Resp Alk


// ***** Diagnosis Phrases - the options correspond to the 10 variables in each code string.
	var phrase = new Array();
	for (i=0;i<10;i++) {
		phrase[i] = new Array();
	}
	phrase[0][0] = "This patient has normal acid-base values. ";
	phrase[0][1] = "These values are close to normal. ";
	phrase[0][2] = "";
	phrase[0][3] = "";
	phrase[0][4] = "";
	phrase[0][5] = "";
	phrase[0][6] = "";
		
	phrase[1][0] = "";
	phrase[1][1] = "The principal abnormality is a ";
	phrase[1][2] = "There is both a ";
	phrase[1][3] = "This patient has both a ";
	phrase[1][4] = "This patient has a ";
		
	phrase[2][0] = "";
	phrase[2][1] = "negligible ";
	phrase[2][2] = "minimal ";
	phrase[2][3] = "mild ";
	phrase[2][4] = "moderate ";
	phrase[2][5] = "marked ";
	phrase[2][6] = "severe ";
		
	phrase[3][0] = "";
	phrase[3][1] = "respiratory ";
	phrase[3][2] = "metabolic ";
		
	phrase[4][0] = "";
	phrase[4][1] = "acidosis ";
	phrase[4][2] = "alkalosis ";
	phrase[4][3] = "compensation ";
		
	phrase[5][0] = "";
	phrase[5][1] = "with ";
	phrase[5][2] = "and ";
		
//	phrase[6][0] = "";
	phrase[6][0] = "no ";
	phrase[6][1] = "negligible ";
	phrase[6][2] = "a minimal ";
	phrase[6][3] = "a mild ";
	phrase[6][4] = "a moderate ";
	phrase[6][5] = "a marked ";
	phrase[6][6] = "a severe ";
		
	phrase[7][0] = "";
	phrase[7][1] = "respiratory ";
	phrase[7][2] = "metabolic ";
		
	phrase[8][0] = "";
	phrase[8][1] = "acidosis";
	phrase[8][2] = "alkalosis";
	phrase[8][3] = "compensation";
		
	phrase[9][0] = "";
	phrase[9][1] = ".";
	phrase[9][2] = " which is typical of chronic respiratory disease.";
	phrase[9][3] = " which compensate completely for each other.";
	phrase[9][4] = " typical of a partially compensated metabolic disturbance.";
	phrase[9][5] = ".  This is typically seen in prolonged hyperventilation.";
	phrase[9][6] = ".  It is typical of an acute respiratory disturbance.";
	phrase[9][7] = " which may occur in prolonged hyperventilation.";

// ***** Severity Break Points - determine the adjectives used in positions 2 and 6 above
// Based on deltaPCO2 values in mmHg. For deltaBE multiply values by pHSlope.
	var severityBreak = new Array();
	severityBreak [0] = 0;												// No
	severityBreak [1] = 3.1;											// Negligible
	severityBreak [2] = 5;												// Minimal
	severityBreak [3] = 8;												// Mild
	severityBreak [4] = 13;												// Moderate
	severityBreak [5] = 21;												// Marked
	severityBreak [6] = 30;												// Severe


//***** Conversions Equations.  Note: only some of these have been tested.
	function PCO2andBEtoBIC() {
		bic = (BE + 30.17) / (0.94292 + 12.569 / PCO2);				// bic approx using Grogono Equation
		for (ii=0;ii<6;ii++)  {											// iterative approximation
			H = BICandPCO2toH();											// Henderson
			bic = (bic + BEandHtoBIC())/2;								// Sig-Anderson
		}
		return bic;														// return bic
	}
	
	function PCO2andBEtoPH() {
		bic = (BE + 30.17) / (0.94292 + 12.569 / PCO2);				// bic approx using Grogono Equation   
		for (ii=0;ii<6;ii++)  {											// iterative approximation
			H = BICandPCO2toH();											// Henderson
			bic = (bic + BEandHtoBIC())/2;								// Sig-Anderson
		}
		return (9-Math.log(H) / 2.302585);								// return pH
	}
	
	function PCO2andBEtoH() {
		bic = (BE + 30.17) / (0.94292 + 12.569 / PCO2);				// bic approx using Grogono Equation
		for (i=0;i<6;i++)  {												// iterative approximation
			H = BICandPCO2toH();											// Henderson
			bic = (bic + BEandHtoBIC())/2;								// Sig-Anderson
		}
		return H;															// return [H+]
	}

	function PCO2andPHtoBE() {
		return 0.9287 * (PCO2 * 24 / Math.exp((9-pH)*2.302585)) + 13.772621 * pH -124.5776754;
	}
	
	function PCO2andPHtoBIC() {
		return (K*PCO2/Math.exp((9-pH)*2.302585));
	}
	
	function PCO2andHtoBE() {
		return 22.2888 *PCO2 / H - 0.624086  - 5.9813759  * Math.log(H);
	}
	
	function PCO2andHtoBIC() {
		return (K*PCO2/H);
	}
	
	function BEandBICtoPCO2() {
		return bic/(Math.exp(BE*0.167185679472339 +0.104338308816056 - bic*0.155265340525961))/24;
	}
	
	function BEandBICtoPH() {
		return BE/13.772621 + 9.0453135536 - bic / 14.83;
	}
	
	function BEandBICtoH() {
		return Math.exp(bic *0.155265340525961 -BE*0.167185679472339 - 0.104338308815997);
	}
	
	function BEandPHtoPCO2() {
		return Math.exp((9-pH)*2.302585) * ((BE -13.772621 * pH +124.577675)/0.9287) / 24;
	}
	
	function BEandPHtobic() {
		return BE/0.9287 - 14.83 * pH +134.142;
	}
	
	function BEandHtoPCO2() {
		return H * (BE /  0.9287 + 0.672 +  6.44058742673995  * Math.log(H) ) / 24;
	}
	
	function BEandHtoBIC() {
		return BE/ 0.9287 + 6.44058742673995 * (Math.log(H)) + 0.672;
	}
	
	function BICandPCO2toBE() {
		return 0.9287 *bic - 0.624086  - 5.9813759  * Math.log(PCO2 * 24 / bic);
	}
	
	function BICandPCO2toPH() {
		return 9 - Math.log(PCO2 * 24 / bic) / 2.302585;
	}
	
	function BICandPCO2toH() {
		return (24*PCO2/bic);
	}
	
	function BICandPHtoBE() {
		return 0.9287 * bic  +  13.772621 * pH -124.5776754;
	}
	
	function BICandPHtoPCO2() {
		return (Math.exp((9-pH)*2.302585) * bic / 24);
	}
	
	function BICandHtoBE() {
		return 0.9287 *bic - 0.624086  - 5.9813759  * Math.log(H);
	}
	
	function BICandHtoPCO2() {
		return (H * bic / 24);
	}
	
	function HtoPH() {
		return (9-Math.log(H) / 2.302585);
	}
	
	function PHtoH() {
		return (Math.exp((9-pH)*2.302585));
	}
	
	function KtoPK() {
		return (9-Math.log(K) / 2.302585);
	}
	
	function PKtoK() {
		return (Math.exp((9-pK)*2.302585));
	}																		// End Deriving Clinical Variable from data. 


// ***** Methods for converting between plot location and clinical variable
	function mxtoPCO2() {												// x-axis to PCO2
		return (mx/scale+10);
	}
	
	function PCO2toxx() {												// PCO2 to x-axis
		return ((PCO2-10) * scale);
	}
	
	function PCO2kPatoxx() {												// PCO2 to x-axis
		return ((PCO2-1.38) * scale /0.138);
	}
	
	function mytoBE() {													// y-axis to BE
		return (30 - my/scale);
	}
	
	function BEtoyy() {													// BE to y-axis
		return ((30 - BE) * scale);  
	}
	
// ***** Draw Isopleths on Graph
function drawIsopleths() {
	drawPHisopleths();													// pH Isopleths
	drawBicarbisopleths();												// Bicarb Isopleths
	drawBEisopleths();													// Base Excess Isopleths
	drawPCO2isopleths();													// CO2 Isoplehts
}

function drawBEisopleths()	{
	context.fillStyle = BEisopleths;									// Color
	context.strokeStyle = BEisopleths;
	context.font = graphFont;											// Font Style
	PCO2 = 10;
	x1 = PCO2toxx();
	for(i=-2;i<3;i++) {													// BE Values
		BE = i*10;
		y1 = BEtoyy() ;
		context.fillText(BE, x1 + 6, y1-3);
		
		context.lineWidth = 1;				
		context.beginPath();
		context.moveTo(0, (y1+0.5));
		context.lineTo(xK[17]-1, (y1+0.5));
		context.stroke();

	}
	BE = 23;
	y1 = BEtoyy()  ;
	context.fillText("SBE", x1 + 6, y1);

}


function drawPCO2isopleths()	{
	context.fillStyle = PCO2isopleths;
	context.strokeStyle = PCO2isopleths;
	context.font = graphFont;											// Color
	context.lineWidth = 1;				
	BE = BELow;										
	y1 = BEtoyy() - 6;
	if (respUnits == "mmHg")	{										// North America Units
		for(i=2;i<10;i++) {												// PCO2 rabge
			PCO2 = i*10;
			x1 = PCO2toxx();												// X value for Line
			if (i<9)	{
				context.fillText(PCO2, x1+4, y1);
			}
			context.beginPath();
			context.moveTo((x1+0.5), 0);
			context.lineTo((x1+0.5), yK[0]-1);
			context.stroke();
		
		}
		PCO2 = 90;															// Location for PCO2
		x1 = PCO2toxx() + 4;
		context.fillText("PCO2", x1, y1);
	}
	else {
		for(i=1;i<13;i++) {												// PCO2 range in kPa
			PCO2 = i;
			x1 = PCO2kPatoxx();											// X value for Line
			if (i<12)	{													// Convert kPa to xx
				context.fillText(PCO2, x1+4, y1);
			}
			context.beginPath();
			context.moveTo((x1+0.5), 0);
			context.lineTo((x1+0.5), yK[0]-1);
			context.stroke();
		
		}
		PCO2 = 12;															// Location for PCO2
		x1 = PCO2kPatoxx() + 4;
		context.fillText("PCO2", x1, y1);
	}
}

function drawPHisopleths()	{
	for (i = 70; i < 78; i++) {											// pH Isopleths
		pH = i / 10;														// 7.0, 7.1, 7.2, etc
		pHdisp = pH.toFixed(1);											// Ensures 7.0 instead of just 7
		PCO2 = PCO2Low;													// Left end of Isopleth
		BE = PCO2andPHtoBE();											// Get height
		x1 = PCO2toxx();													// PCO2 to x value
		y1 = BEtoyy();													// BE to y value

		PCO2 = PCO2High;													// Right end Isopleth
		BE = PCO2andPHtoBE();											// Get height
		if (BE > BEHigh) {												// If outside edge
			BE = BEHigh;													// Use edge
			PCO2 = BEandPHtoPCO2();										// Get CO2 at edge
		}
		x2 = PCO2toxx();													// PCO2 to x value
		y2 = BEtoyy();													// BE to y value
		
		context.lineWidth = 1;				
		context.strokeStyle = pHisopleths;
		context.beginPath();
		context.moveTo(x1, y1);
		context.lineTo(x2, y2);
		context.stroke();
		
		context.fillStyle = pHisopleths;
		context.font = graphFont;
		if (i<74){
			context.fillText(pHdisp, x2-scale*4, y2 + scale*3 + (i-70) * scale / 3);
		}
		else {
			context.fillText(pHdisp, x2-11*scale + (i-72)*scale, y2 + 2.5 * scale +2);
		}
	}
	BE = 30;																// BE Near Bottom
	y2 = BEtoyy() ;														// y Plot value
	PCO2 = 100;															// PCO2 Near Top
	x2 = PCO2toxx();														// x Plot value
	context.fillText("pH", x2-scale*4, y2 + 2.5 * scale +2);		// Print "pH"
}

function drawBicarbisopleths()	{
	for (j = 8; j > 0; j--)  {											// Eight steps across screen
		for (i = 1; i < 10; i++) {										// Nine Bicarb Isopleths
			PCO2 = j * 10 + 2;											// 10, 20, 30, 40, etc
			x2 = PCO2toxx();												// x value Left
			bic = i * 5;													// bic
			BE = BICandPCO2toBE();										// get Bic
			y2 = BEtoyy();												// y value
			
			if (BE > BEHigh)  {											// Too High
				BE = BEHigh;												// bic at bottom
				y2 = BEtoyy() ;											// y Plot value
				PCO2 = BEandBICtoPCO2();								// get PCO2 at bottom
				x2 = PCO2toxx();											// x Plot value
			}
				
			if (BE < BELow)  {											// Too High
				BE = BELow;												// bic at bottom
				y2 = BEtoyy();											// y Plot value
				PCO2 = BEandBICtoPCO2();								// get PCO2 at bottom
				x2 = PCO2toxx();											// x Plot value
			}
				
			PCO2 = (j + 1) * 10 - 2;									// 10, 20, 30, 40, etc
			x1 = PCO2toxx();												// x value Left
			BE = BICandPCO2toBE();										// get Bic
			y1 = BEtoyy();												// y value
				
			if (BE < BELow)  {											// Too Low
				BE = BELow;												// bic at bottom
				y1 = BEtoyy();											// y Plot value
				PCO2 = BEandBICtoPCO2();								// get PCO2 at bottom
				x1 = PCO2toxx();											// x Plot value
			}

			context.lineWidth = 1.5;				
			context.strokeStyle = BICisopleths;
			context.beginPath();
			context.moveTo(x1, y1);
			context.lineTo(x2, y2);
			context.stroke();
			
			context.fillStyle = BICisopleths;
			context.font = graphFont;
			
			if ((j == 8) && (i>1))  {									// Alternate Gap for Numbers			
			 	context.fillText(bic, x1-4*scale, y2+2.5 * scale);
			}
		}
	}
	bic = 45;
	PCO2 = 84;																// PCO2 Near Top
	BE = BICandPCO2toBE()												// Get BE level
	y2 = BEtoyy() - 4 ;													// y Plot value
	x2 = PCO2toxx() - 2;													// x Plot value
	context.fillText("Bic", x2, y2);									// Print "Bic"
}

function changeColors()	{												// Color Changes Beside the Graph

	if (PCO2Change>3)	 {													// PCO2 changes Alkalosis and Acidosis
		document.getElementById('RAc').style.color = AcidText;
		document.getElementById('RAlk').style.color = NeutralText;
	}
	else if (PCO2Change<-3) {
		document.getElementById('RAc').style.color = NeutralText;
		document.getElementById('RAlk').style.color = AlkaliText;
	}
	else	{
		document.getElementById('RAc').style.color = NeutralText;
		document.getElementById('RAlk').style.color = NeutralText;
	}

	if (BE > 2) { 														// BE changes Alkalosis and Acidosis
		document.getElementById('MAlk').style.color = AlkaliText;
		document.getElementById('MAc').style.color = NeutralText;
	}
	else if (BE > -2) {
		document.getElementById('MAlk').style.color = NeutralText;
		document.getElementById('MAc').style.color = NeutralText;
	}
	else if (BE > -30) {
		document.getElementById('MAlk').style.color = NeutralText;
		document.getElementById('MAc').style.color = AcidText;
	}

}

// ***** Return 'true' if the point 'pt' is inside the polygon 'main'.
function isPointInPoly(main, pt) {
    for(var c = false, i = -1, l = main.length, j = l - 1; ++i < l; j = i)
        ((main[i].y <= pt.y && pt.y < main[j].y) || (main[j].y <= pt.y && pt.y < main[i].y))
        && (pt.x < (main[j].x - main[i].x) * (pt.y - main[i].y) / (main[j].y - main[i].y) + main[i].x)
        && (c = !c);
    return c;
}

// ***** File Polygons.
function fillPolygon(main, fillCol, strokeCol)
{
	context.restore();													// **** Added to achieve Uniform Radial Brightness
	context.save();														// **** Added to achieve Uniform Radial Brightness
	context.beginPath();
	context.moveTo(main[0].x, main[0].y);
	for (var i = 1; i < (main.length); i++) { 
		context.lineTo(main[i].x, main[i].y) 							// Outline
	}
	context.clip();														// **** Added to achieve Uniform Radial Brightness
	context.fillStyle = fillCol;
	context.fill();
	context.closePath();

	context.beginPath();
	context.moveTo(main[0].x, main[0].y);
	for (var i = 1; i < main.length; i++) { 
		context.lineTo(main[i].x, main[i].y) 
	context.lineWidth = 1;				
	context.strokeStyle = strokeCol;
	context.stroke();
	}
	
	context.lineWidth = 2;				
	context.strokeStyle = graphBorder;
	context.strokeRect(2,2,xK[17]-3,yK[0]-3);
}

//	***** Mouse move 'event' gives x,y position.
//	Coordinates in rectangle and finds polygon that encloses the position, if any.
function doMouse(event) {
	var rect = canvas.getBoundingClientRect();
	mx = event.pageX - rect.left;
	my = event.pageY - rect.top;
	
// Scan all polygons, highlighting the one that contains the event.
	reportStringdisp = "";												// Start Text Description
	for (p = 0; p < polygons.length; p++)
	{
		if (isPointInPoly(polygons[p], {x: mx, y: my})) {			// First Test is Mouse in Clinical Typical Zone

// reportStringdisp = "Zone = " + p + ". ";												// Provide Zone for DeBugging.

			if((p!=2)&&(p!=4)&&(p!=6)&&(p!=8)&&(p!=9)&&(p!=11)&&(p!=13)&&(p!=15)&&(p!=17)&&(p!=18)) {	
				if ((p > 4) && (p < 13)) {								// Find to Mouse in Acid Range
					color1 = AcidColor;									// Color Red
					color2 = AcidBorderColor;							// Color Red
				}
				else	{													// Else Mouse in Alkaline Range
					color1 = AlkaliColor;								// Color Blue
					color2 = AlkaliBorderColor;							// Color Blue
				}
			}
			else {
				color1 = graphOtherZone;								// Color Grey
				color2 = graphOtherZone;								// Color Grey
			}

			fillPolygon(polygons[p], color1, color2);					// Fill Windmill Spokes			
			drawIsopleths();												// PCO2, BE, pH, Bic
			
			PCO2 = Math.round(mxtoPCO2());								// Required for Following Conversions
			BE = (mytoBE());												// Required for Following Conversions
			PCO2Change = PCO2 - 40;										// Change in PCO2 + and -
			deltaPCO2 = Math.abs(PCO2-40);								// Absolute Change in PCO2
			deltaBE = Math.abs(BE);										// Change in BE
			
			Hdisp = Math.round(PCO2andBEtoH());						// [H+] Display
			pHdisp = PCO2andBEtoPH().toFixed(2);						// pH Display - Two Dec
			bicdisp = Math.round(PCO2andBEtoBIC());					// Bicard Display
			
			if (respUnits == "mmHg")	{								// North America Units
				PCO2disp = Math.round(mxtoPCO2());						// PCO2 mmHg
			}
			else {															// Europe Units
				PCO2disp = (Math.round(PCO2*1.38)/10).toFixed(1);	// PCO2 Converted to kiloPascals
			}
			
			BEdisp = (Math.round(10*mytoBE())/10).toFixed(1);		// Standard Base Excess One Decimal
			if (BEdisp>=0) {BEdisp = "&nbsp; " + BEdisp;}			// Space precedes non-negatives
			if (Math.abs(BEdisp)<10) {BEdisp="&nbsp; " + BEdisp;}	// Space precedes small numbers

			for (i=0;i<7;i++)	{											// Get Resp Magnitude
				if (deltaPCO2 > severityBreak[i]) {
					respMagn = i;
				}
				if (deltaBE > pHSlope * severityBreak[i]) {			// Get Metabolic Magnitude
					metMagn = i;
				}
			}
			if (respMagn > metMagn) {									// Find Dominant
				temp1 = respMagn;											// And Assign to Temp 1
				temp2 = metMagn;											// Assign Other to Temp 2
			}
			else	{														// Reverse
				temp1 = metMagn;
				temp2 = respMagn;
			}
			if (temp1 == 0)	{											// Near Center - Normal - No Other Reports
				reportStringdisp += phrase[0][temp1];
			}
			else {															// Use Code String to Select Phrases
				for (i=1;i<2;i++)	{
					reportStringdisp += phrase[i][codes[p].charAt(i)];
				}
				reportStringdisp += phrase[i][temp1];
				for (i=3;i<6;i++)	{
					reportStringdisp += phrase[i][codes[p].charAt(i)];
				}
				reportStringdisp += phrase[i][temp2];
				for (i=7;i<10;i++)	{
					reportStringdisp += phrase[i][codes[p].charAt(i)];
				}
			}

	
			document.getElementById('Report').innerHTML = reportStringdisp;
			document.getElementById('pH').innerHTML = "pH = <br>" + pHdisp;
			document.getElementById('PCO2').innerHTML = "PCO2 =<br>" + PCO2disp + "<br>" + respUnits;
			document.getElementById('SBE').innerHTML = "SBE =<br>" + BEdisp + "<br>" + metUnits;
			document.getElementById('Bic').innerHTML = "Bic =<br>" + bicdisp + "<br>" + metUnits;
			changeColors();													// Colors of Labels at edges.
		}
		else {
			fillPolygon(polygons[p], graphBackground, graphBackgroundBorder);
			drawIsopleths();													// PCO2, BE, pH, Bic
			changeColors();													// Colors of Labels at edges.
		}
	}
}
  
// Display the polygons and wait for mouse events.  
function displayPolygons() {
	canvas = document.getElementById('main');
	context = canvas.getContext('2d');
	for (p = 0; p < polygons.length; p++) {
		fillPolygon(polygons[p], graphBackground, graphBackgroundBorder);		// Fill polygons, Background, Border
	}
	canvas.addEventListener ('mousemove', doMouse, false);
	drawIsopleths();														// PCO2, BE, pH, Bic
	canvas.addEventListener ("mouseout", doMouse, false);			// Clear Highlighting on Exit on MouseOut
}

// Invoke the polygon displayer when the page is loaded.
	window.addEventListener('load', displayPolygons, false);

*/

namespace AcidBaseLibrary
{
    public class Diagram
    {
        static public Tuple<double, double> GetData(double ph, double pco2)
        {
            if (ph < 6.43 || ph > 8.26)
            {
                throw new ArgumentException($"{nameof(ph)} only have values [6.43, 8.26]");
            }

            if (pco2 < 10.0 || pco2 > 100.0)
            {
                throw new ArgumentException($"{nameof(pco2)} only have values [10.0, 100.0]");
            }

            return new Tuple<double, double>(0.0, 24.0);
        }

        static public Bitmap GetBitmap()
        {
            return new Bitmap(400, 400);
        }
    }
}
