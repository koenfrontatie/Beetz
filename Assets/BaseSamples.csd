;<Cabbage>
;form caption("Dist Slider")
;hslider channel("distortion"), text("Dist Slider"), range(0, 10, 0)
;</Cabbage>
<CsoundSynthesizer>
<CsOptions>
-odac ; activate real-time audio output
</CsOptions>
<CsInstruments>
sr 	= 	4410
kr = 10
ksmps 	= 	441
nchnls 	= 	2	

chn_k "distortion", 1

instr 1
    kSpeed init 1     
    
	a1 diskin2 "BaseSamples/1kick.wav", kSpeed, 0, 0

    kDistortion chnget "distortion"

    aDist distort1 a1, kDistortion, 10, .4, 0, 0

    outs      aDist, aDist     
endin

instr 2 
	kSpeed init 1      
    
	a1 diskin2  "BaseSamples/2hat.wav", kSpeed, 0, 0

    kDistortion chnget "distortion"

    aDist distort1 a1, kDistortion, 10, .4, 0, 0

    outs      aDist, aDist         
endin
  
instr 3 
	kSpeed  init     1           
    
	a1 diskin2  "BaseSamples/3clap.wav", kSpeed, 0, 0

    kDistortion chnget "distortion"

    aDist distort1 a1, kDistortion, 10, .4, 0, 0

    outs      aDist, aDist       
endin
  
instr	4 
	kSpeed  init     1          

	a1      diskin2  "BaseSamples/4cow.wav", kSpeed, 0, 0
    
	kDistortion chnget "distortion"

    aDist distort1 a1, kDistortion, 10, .4, 0, 0

    outs      aDist, aDist         
endin
  
instr	5 ; 
	kSpeed  init     1          

	a1      diskin2  "BaseSamples/5snare.wav", kSpeed, 0, 0
	
	kDistortion chnget "distortion"

    aDist distort1 a1, kDistortion, 10, .4, 0, 0

    outs      aDist, aDist         
endin
  
instr	6 

	kSpeed  init     1           

	a1      diskin2  "BaseSamples/6kick808.wav", kSpeed, 0, 0
    
	kDistortion chnget "distortion"

    aDist distort1 a1, kDistortion, 10, .4, 0, 0

    outs      aDist, aDist        
  endin
  
instr	7 
	kSpeed  init     1           

	a1      diskin2  "BaseSamples/7tab05.wav", kSpeed, 0, 0
    
	kDistortion chnget "distortion"

    aDist distort1 a1, kDistortion, 10, .4, 0, 0

    outs      aDist, aDist  
endin
  
instr	8 
    kSpeed  init     1           

    a1      diskin2  "BaseSamples/8khat3.wav", kSpeed, 0, 0
        
    kDistortion chnget "distortion"

    aDist distort1 a1, kDistortion, 10, .4, 0, 0

    outs      aDist, aDist  
endin
  
instr	9 
    kSpeed  init     1          

    a1      diskin2  "BaseSamples/9walk.wav", kSpeed, 0, 0
    
    kDistortion chnget "distortion"

    aDist distort1 a1, kDistortion, 10, .4, 0, 0

    outs      aDist, aDist  
endin
  
instr	10
    kSpeed  init     1

    a1      diskin2  "BaseSamples/10chant.wav", kSpeed, 0, 0
    
    kDistortion chnget "distortion"

    aDist distort1 a1, kDistortion, 10, .4, 0, 0

    outs      aDist, aDist  
endin

</CsInstruments>

<CsScore>

f0 z 

</CsScore>
</CsoundSynthesizer>