;<Cabbage>
;form caption("Dist Slider")
;hslider channel("distortion"), text("Dist Slider"), range(0, 10, 0)
;</Cabbage>
<CsoundSynthesizer>
<CsOptions>
;-odac ; activate real-time audio output
-o RenderKUTUnity.wav -f

</CsOptions>
<CsInstruments>
sr 	= 	4410
kr = 10
ksmps 	= 	441
nchnls 	= 	2	

instr 1
    kSpeed init 1     
    
	a1 diskin2 "BaseSamples/1kick.wav", kSpeed, 0, 0



    outs      a1, a1     
endin

</CsInstruments>

<CsScore>
i 1 0 1

;f0 z 

</CsScore>
</CsoundSynthesizer>