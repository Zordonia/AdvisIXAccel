using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using ZedGraph;

namespace DockingAnalytics
{

    public class _xmlFile
    {
        private _dsSentry_data Sentry_data = new _dsSentry_data();
        private string _fileName, _encoding;
        private double _xmlVersion;
        private decimal[] dcmACCVals;
        private double[] dblAccVals;
        private float[] floatAcclVals;
        private PointPairList accList = new PointPairList();
        private double accXScale = 1, accYScale = 1, accYOffset = 0;


        public _xmlFile(String fileName)
        {
            _fileName = fileName;
        }

        public void AddAccelerationValues(System.Collections.ArrayList values)
        {
            this.dsSentry_data.values = new List<decimal>();
            decimal temp;
            foreach (object sh in values)
            {
                temp = Convert.ToDecimal(sh);
                this.dsSentry_data.values.Add(temp);
            }
        }

        private List<double> parseFFTValues(string values)
        {
            values = values.Replace("\r\n", "\n");
            string[] sepVals = values.Split('\n');
            List<double> fftVals = new List<double>();
            for (int i = 0; i < sepVals.Length; i++)
            {
                fftVals.Add(Convert.ToDouble(sepVals[i]));
            }
            return fftVals;
        }

        private List<decimal> parseACCValues(PointPairList Alist, string values)
        {
            double t = 0;
            values = values.Replace("\r\n", "\n");
            string[] sepVals = values.Split(new char[] { '\n', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            dcmACCVals = new decimal[sepVals.Length];
            floatAcclVals = new float[sepVals.Length];
            //powrVals = new float[sepVals.Length];
            double tempfs = 1.0 / this.dsSentry_data.sampling_Freq;
            if (double.IsInfinity(tempfs))
            {
                tempfs = 1;
            }

            double temfes = tempfs / ((sepVals.Length) * 2);
            double ffreq = 0.0;
            // TODO Remove?
            accYOffset = 0;
            // accelValues is now a string containing the values for each of the accelerations in g's
            for (double q = 0.0; q < sepVals.Length; q += 1)
            {
                //int qi = Convert.ToInt32(q);
                t = q * tempfs;
                ffreq = q * tempfs;
                try
                {
                    int g = (int)q;
                    //Console.WriteLine(Convert.ToDouble(sepVals[g]));
                    dcmACCVals[g] = Convert.ToDecimal(sepVals[g]) * (decimal)accYScale + (decimal)accYOffset;
                    floatAcclVals[g] = Math.Abs((float)dcmACCVals[g] * (float)accYScale + (float)accYOffset);
                    Alist.Add(ffreq * accXScale, Convert.ToDouble(sepVals[g]) * accYScale + accYOffset);
                }
                catch (FormatException e)
                {

                }
            }
            return dcmACCVals.ToList<decimal>();
        }

        public void readXML()
        {
            XmlTextReader reader = new XmlTextReader(fileName);
            string values = "";
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "dsSentry":
                            //if (reader.HasAttributes)
                            //{
                            reader.ReadToFollowing("sentry_info");
                            this.dsSentry_data.manufacturer_Name = reader.GetAttribute("manufacturer_Name");
                            this.dsSentry_data.gateway_Serial = reader.GetAttribute("gateway_Serial");
                            this.dsSentry_data.node_Serial = reader.GetAttribute("node_Serial");
                            this.dsSentry_data.owner_Name = reader.GetAttribute("owner_Name");
                            this.dsSentry_data.location_Name_1 = reader.GetAttribute("location_Name_1");
                            this.dsSentry_data.location_Name_2 = reader.GetAttribute("location_Name_2");
                            this.dsSentry_data.location_Name_3 = reader.GetAttribute("location_Name_3");
                            this.dsSentry_data.gps_Longitude = reader.GetAttribute("gps_Longitude");
                            this.dsSentry_data.gps_Latitude = reader.GetAttribute("gps_Latitude");
                            // Need method for creating dateTime object from timestamp
                            DateTime sdts, sdte;
                            DateTime.TryParse(reader.GetAttribute("settings_Timestamp_Created"), out sdts);
                            DateTime.TryParse(reader.GetAttribute("settings_Timestamp_LastEdit"), out sdte);
                            this.dsSentry_data.timestamp_End = sdte;
                            this.dsSentry_data.timestamp_Start = sdts;
                            this.dsSentry_data.sampling_Freq = Convert.ToDouble(reader.GetAttribute("sampling_Freq"));
                            this.dsSentry_data.max_Res_Freq = Convert.ToDouble(reader.GetAttribute("max_Res_Freq"));
                            this.dsSentry_data.offset_Calibration = Convert.ToDouble(reader.GetAttribute("offset_Calibration"));
                            this.dsSentry_data.accel_xCalibration = Convert.ToDouble(reader.GetAttribute("accel_xCalibration"));
                            this.dsSentry_data.accel_yCalibration = Convert.ToDouble(reader.GetAttribute("accel_yCalibration"));
                            this.dsSentry_data.sched_capture_freq = reader.GetAttribute("scheduled_Capture_Freq");

                            // was boolean
                            this.dsSentry_data.sclk_inversion = reader.GetAttribute("sclk_Inversion");
                            this.dsSentry_data.spi_clk_format = reader.GetAttribute("spi_Clk_Format");
                            this.dsSentry_data.data_first_bit_sel = reader.GetAttribute("data_First_Bit_Selector");
                            this.dsSentry_data.coeff_mem_ct_reset = reader.GetAttribute("coeff_Memory_Count_Reset");
                            this.dsSentry_data.nonoverlap_clk_sel = reader.GetAttribute("nonoverlap_Clk_Selector");
                            this.dsSentry_data.ota_output_buf_enable = reader.GetAttribute("ota_Output_Buffer_Enable");
                            this.dsSentry_data.sf_output_buf_enable = reader.GetAttribute("sf_Output_Buffer_Enable");
                            this.dsSentry_data.input_buf_enable = reader.GetAttribute("input_Buffer_Enable");
                            this.dsSentry_data.real_coeff_mag = reader.GetAttribute("real_Coeff_Magnitude");
                            this.dsSentry_data.real_coeff_sign = reader.GetAttribute("real_Coeff_Sign");
                            this.dsSentry_data.quad_coeff_mag = reader.GetAttribute("quad_Coeff_Magnitude");
                            this.dsSentry_data.quad_coeff_sign = reader.GetAttribute("quad_Coeff_Sign");
                            this.dsSentry_data.sf_output_buf_sel = reader.GetAttribute("sf_Output_Buffer_Selector");
                            this.dsSentry_data.ota_output_buf_sel = reader.GetAttribute("ota_Output_Buffer_Selector");
                            this.dsSentry_data.sentry_enable_bit = reader.GetAttribute("sentry_Enable_Bit");
                            this.dsSentry_data.pll_bias_sel = reader.GetAttribute("pll_Bias_Selector");
                            this.dsSentry_data.sentry_sensitivity = reader.GetAttribute("sentry_Sensitivity");
                            this.dsSentry_data.sentry_clk_src = reader.GetAttribute("sentry_Clk_Source");
                            this.dsSentry_data.clk_out_sel = reader.GetAttribute("clk_Out_Selector");
                            this.dsSentry_data.eff_sample_rate = reader.GetAttribute("effective_Sample_Rate");
                            this.dsSentry_data.decimator_clk_enable = reader.GetAttribute("decimator_Clk_Enable");
                            this.dsSentry_data.decimator_reset = reader.GetAttribute("decimator_Reset");
                            this.dsSentry_data.sig_del_input_mode = reader.GetAttribute("sigma_Delta_Input_Mode");
                            this.dsSentry_data.dc_ss_mod = reader.GetAttribute("spread_Spectrum_Modulator_Dc_Voltage");
                            this.dsSentry_data.adc_ss_mod_sel = reader.GetAttribute("adc_Spread_Spectrum_Modulator_Selector");
                            this.dsSentry_data.dig_out_test_sig_sel = reader.GetAttribute("dig_Out_Test_Signal_Selector");

                            this.dsSentry_data.pll_div_m_reg = Convert.ToDouble(reader.GetAttribute("pll_Divider_M_Register"));
                            this.dsSentry_data.pll_div_n_reg = Convert.ToDouble(reader.GetAttribute("pll_Divider_N_Register"));
                            this.dsSentry_data.num_quant_level = Convert.ToDouble(reader.GetAttribute("num_Quant_Level_Utilized"));
                            this.dsSentry_data.sentry_status = Convert.ToDouble(reader.GetAttribute("sentry_Status"));
                            this.dsSentry_data.atten_pin_ctrl = Convert.ToDouble(reader.GetAttribute("attenuation_Pin_Ctrl"));
                            this.dsSentry_data.nfet_gate_ctrl = Convert.ToDouble(reader.GetAttribute("nfet_Gate_Ctrl"));
                            this.dsSentry_data.org_sentry_dig_test_output = Convert.ToDouble(reader.GetAttribute("sentry_Digital_Test_Output_Origin"));
                            this.dsSentry_data.sig_sel_ss_demod = Convert.ToDouble(reader.GetAttribute("spread_Spectrum_Demodulator_Signal_Selector"));
                            this.dsSentry_data.opt_Field_1 = reader.GetAttribute("opt_Field_1");
                            this.dsSentry_data.opt_Field_2 = reader.GetAttribute("opt_Field_2");
                            this.dsSentry_data.opt_Field_3 = reader.GetAttribute("opt_Field_3");
                            this.dsSentry_data.opt_Field_4 = reader.GetAttribute("opt_Field_4");
                            this.dsSentry_data.opt_Field_5 = reader.GetAttribute("opt_Field_5");
                            this.dsSentry_data.opt_Field_6 = reader.GetAttribute("opt_Field_6");
                            this.dsSentry_data.opt_Field_7 = reader.GetAttribute("opt_Field_7");
                            this.dsSentry_data.opt_Field_8 = reader.GetAttribute("opt_Field_8");

                            reader.ReadToFollowing("sentry_data");
                            this.dsSentry_data.node_Serial = reader.GetAttribute("node_Serial");
                            this.dsSentry_data.capture_type = reader.GetAttribute("capture_Type");
                            //this.fileName = reader.GetAttribute("file_Name");

                            // Need method for creating dateTime object from timestamp
                            DateTime dts, dte, bct;
                            DateTime.TryParse(reader.GetAttribute("timestamp_Start"), out dts);
                            DateTime.TryParse(reader.GetAttribute("timestamp_End"), out dte);
                            this.dsSentry_data.timestamp_End = dte;
                            this.dsSentry_data.timestamp_Start = dts;

                            // Need method for creating dateTime object from timestamp
                            uint nBands = Convert.ToUInt16(reader.GetAttribute("num_Bands"));
                            this.dsSentry_data.numBands = nBands;

                            this.dsSentry_data.num_captures_battery = Convert.ToDouble(reader.GetAttribute("num_Captures_Battery"));
                            this.dsSentry_data.num_alarms_battery = Convert.ToDouble(reader.GetAttribute("num_Alarms_Battery"));
                            DateTime.TryParse(reader.GetAttribute("battery_Change_Timestamp"), out bct);
                            this.dsSentry_data.battery_change_timestamp = bct;
                            this.dsSentry_data.sensor_temp = reader.GetAttribute("sensor_Temp");

                            // acceleration data
                            accXScale = this.dsSentry_data.accel_xCalibration;
                            accYScale = this.dsSentry_data.accel_yCalibration;
                            accYOffset = this.dsSentry_data.offset_Calibration;
                            if (accXScale == 0) { accXScale = 1; }
                            if (accYScale == 0) { accYScale = 1; }

                            reader.Read();

                            if (reader.NodeType == System.Xml.XmlNodeType.Text)
                            {
                                values = reader.Value;
                                this.dsSentry_data.values = parseACCValues(accList, values);
                                this.dsSentry_data.ACCPPL = accList;
                            }



                            //using (XmlReader bandreader = reader.ReadSubtree())
                            //{
                            //reader.Read(); // Need this to get to bands 
                            for (int i = 0; i < nBands; i++)
                            {
                                //bandreader.ReadStartElement("band");
                                reader.ReadToFollowing("band_data");
                                //bandreader.Read();
                                //bandreader.Read();
                                _xmlFile._dsSentry_data._band band = new _dsSentry_data._band();
                                DateTime tmp;
                                DateTime.TryParse(reader.GetAttribute("timestamp_Created"), out tmp);
                                band.TimeStampCreated = tmp;
                                DateTime.TryParse(reader.GetAttribute("timestamp_LastEdit"), out tmp);
                                band.TimeStampLastEdit = tmp;
                                band.description = reader.GetAttribute("description");
                                band.center_Freq = Convert.ToDouble(reader.GetAttribute("center_Freq"));
                                band.bandwidth = Convert.ToDouble(reader.GetAttribute("bandwidth"));
                                band.threshold_Level = Convert.ToDouble(reader.GetAttribute("threshold_Level"));
                                band.quant_Level = Convert.ToUInt16(reader.GetAttribute("quant_Level"));
                                band.max_Reg_Val = Convert.ToDouble(reader.GetAttribute("max_Reg_Val"));
                                band.alarm_State = Convert.ToBoolean(reader.GetAttribute("alarm_State"));
                                this.dsSentry_data.band.times_integrate_pwr = Convert.ToDouble(reader.GetAttribute("times_To_Integrate_Power"));
                                this.dsSentry_data.band.times_thres_exceed_alarm_trig = Convert.ToDouble(reader.GetAttribute("times_Threshold_Exceed_Alarm_Trigger"));
                                this.dsSentry_data.band.times_repeat_ref_sig = Convert.ToDouble(reader.GetAttribute("times_To_Repeat_Reference_Signal"));
                                this.dsSentry_data.bandList.Add(band);
                            }
                            //}


                            reader.ReadToFollowing("reference_data");
                            this.dsSentry_data.lut_data = Convert.ToDouble(reader.GetAttribute("lut_Data"));
                            this.dsSentry_data.ref_func_mem_map_1 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_1"));
                            this.dsSentry_data.ref_func_mem_map_2 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_2"));
                            this.dsSentry_data.ref_func_mem_map_3 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_3"));
                            this.dsSentry_data.ref_func_mem_map_4 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_4"));
                            this.dsSentry_data.ref_func_mem_map_5 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_5"));
                            this.dsSentry_data.ref_func_mem_map_6 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_6"));
                            this.dsSentry_data.ref_func_mem_map_7 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_7"));
                            this.dsSentry_data.ref_func_mem_map_8 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_8"));
                            this.dsSentry_data.sig_del_mod_data_out_reg = Convert.ToDouble(reader.GetAttribute("sigma_Delta_Modulator_Data_Out_Register"));

                            //}
                            break;
                        #region
                        /*
                        case "accel_Values":
                            if (reader.HasAttributes)
                            {
                                // Need method for creating dateTime object from timestamp
                                DateTime dts, dte;
                                DateTime.TryParse(reader.GetAttribute("timestamp_Start"), out dts);
                                DateTime.TryParse(reader.GetAttribute("timestamp_End"), out dte);
                                this.dsSentry_data.accel_Values.timestamp_Start = dte;
                                this.dsSentry_data.accel_Values.timestamp_End = dts;
                                this.dsSentry_data.accel_Values.freq = Convert.ToDouble(reader.GetAttribute("freq"));
                                this.dsSentry_data.accel_Values.freq_Scale = reader.GetAttribute("freq_Scale");
                                this.dsSentry_data.accel_Values.type_id = reader.GetAttribute("type_id");
                                this.dsSentry_data.accel_Values.name = reader.GetAttribute("name");
                                this.dsSentry_data.accel_Values.description = reader.GetAttribute("description");
                                this.dsSentry_data.accel_Values.sample_rate = reader.GetAttribute("sample_rate");
                                this.dsSentry_data.accel_Values.eng_units = reader.GetAttribute("eng_units");
                                this.dsSentry_data.accel_Values.warning_min = Convert.ToDouble(reader.GetAttribute("warning_min"));
                                this.dsSentry_data.accel_Values.warning_max = Convert.ToDouble(reader.GetAttribute("warning_max"));
                                this.dsSentry_data.accel_Values.alarm_min = Convert.ToDouble(reader.GetAttribute("alarm_min"));
                                this.dsSentry_data.accel_Values.alarm_max = Convert.ToDouble(reader.GetAttribute("alarm_max"));
                                this.dsSentry_data.accel_Values.scaling_type = reader.GetAttribute("scaling_type");
                                this.dsSentry_data.accel_Values.tach_id = reader.GetAttribute("tach_id");
                                this.dsSentry_data.accel_Values.tach_speed = Convert.ToDouble(reader.GetAttribute("tach_speed"));
                                this.dsSentry_data.accel_Values.acc_XScale = Convert.ToDouble(reader.GetAttribute("acc_XScale"));
                                this.dsSentry_data.accel_Values.acc_YScale = Convert.ToDouble(reader.GetAttribute("acc_YScale"));
                                this.dsSentry_data.accel_Values.acc_YOffset = Convert.ToDouble(reader.GetAttribute("acc_YOffset"));
                                this.dsSentry_data.accel_Values.maxResFreq = Convert.ToDouble(reader.GetAttribute("max_Res_Freq"));
                                accXScale = this.dsSentry_data.accel_Values.acc_XScale;
                                accYScale = this.dsSentry_data.accel_Values.acc_YScale;
                                accYOffset = this.dsSentry_data.accel_Values.acc_YOffset;
                                if (accXScale == 0) { accXScale = 1; }
                                if (accYScale == 0) { accYScale = 1; }
                                reader.Read();
                                if (reader.NodeType == System.Xml.XmlNodeType.Text)
                                {
                                    values = reader.Value;
                                    this.dsSentry_data.accel_Values.values = parseACCValues(accList, values);
                                    this.dsSentry_data.accel_Values.ACCPPL = accList;
                                }
                            }
                            break;
                        case "fft_Values":
                            if (reader.HasAttributes)
                            {
                                // Need method for creating dateTime object from timestamp
                                this.dsSentry_data.fft_Values.timestamp_Start = new DateTime();// reader.GetAttribute("timestamp_Start");
                                this.dsSentry_data.fft_Values.timestamp_End = new DateTime();// reader.GetAttribute("timestamp_End");
                                this.dsSentry_data.fft_Values.freq = Convert.ToDouble(reader.GetAttribute("freq"));
                                this.dsSentry_data.fft_Values.freq_Scale = reader.GetAttribute("freq_Scale");
                                this.dsSentry_data.fft_Values.type_id = reader.GetAttribute("type_id");
                                this.dsSentry_data.fft_Values.name = reader.GetAttribute("name");
                                this.dsSentry_data.fft_Values.description = reader.GetAttribute("description");
                                this.dsSentry_data.fft_Values.eng_units = reader.GetAttribute("eng_units");
                                this.dsSentry_data.fft_Values.warning_min = Convert.ToDouble(reader.GetAttribute("warning_min"));
                                this.dsSentry_data.fft_Values.warning_max = Convert.ToDouble(reader.GetAttribute("warning_max"));
                                this.dsSentry_data.fft_Values.alarm_min = Convert.ToDouble(reader.GetAttribute("alarm_min"));
                                this.dsSentry_data.fft_Values.alarm_max = Convert.ToDouble(reader.GetAttribute("alarm_max"));
                                this.dsSentry_data.fft_Values.scaling_type = reader.GetAttribute("scaling_type");
                                this.dsSentry_data.fft_Values.tach_id = reader.GetAttribute("tach_id");
                                this.dsSentry_data.fft_Values.tach_speed = Convert.ToDouble(reader.GetAttribute("tach_speed"));
                                this.dsSentry_data.fft_Values.fmax = Convert.ToDouble(reader.GetAttribute("fmax"));
                                this.dsSentry_data.fft_Values.timer_max = Convert.ToDouble(reader.GetAttribute("timer_max"));
                                this.dsSentry_data.fft_Values.fmin = Convert.ToDouble(reader.GetAttribute("fmin"));
                                this.dsSentry_data.fft_Values.fft_XScale = Convert.ToDouble(reader.GetAttribute("fft_XScale"));
                                this.dsSentry_data.fft_Values.fft_YScale = Convert.ToDouble(reader.GetAttribute("fft_YScale"));
                                reader.Read();
                                if (reader.NodeType == System.Xml.XmlNodeType.Text)
                                {
                                    values = reader.Value;
                                    // TODO: Is this necessary?
                                    //this.dsSentry_data.fft_Values.values = parseFFTValues(values);
                                }
                            }
                            break;
                        case "fft_bands":
                            Console.WriteLine(reader.NodeType);
                            if (reader.HasAttributes)
                            {
                                // Need method for creating dateTime object from timestamp
                                uint nBands = Convert.ToUInt16(reader.GetAttribute("num_Bands"));
                                this.dsSentry_data.fft_bands.numBands = nBands;
                                using (XmlReader bandreader = reader.ReadSubtree())
                                {
                                    bandreader.Read(); // Need this to get to bands 
                                    for (int i = 0; i < nBands; i++)
                                    {
                                        //bandreader.ReadStartElement("band");
                                        bandreader.ReadToFollowing("band");
                                        //bandreader.Read();
                                        //bandreader.Read();
                                        _xmlFile._dsSentry_data._band band = new _dsSentry_data._band();
                                        DateTime tmp;
                                        DateTime.TryParse(bandreader.GetAttribute("timestamp_Created"), out tmp);
                                        band.TimeStampCreated = tmp;
                                        DateTime.TryParse(bandreader.GetAttribute("timestamp_LastEdit"), out tmp);
                                        band.TimeStampLastEdit = tmp;
                                        band.description = bandreader.GetAttribute("description");
                                        band.center_freq = Convert.ToDouble(bandreader.GetAttribute("center_Freq"));
                                        band.threshold = Convert.ToDouble(bandreader.GetAttribute("threshold"));
                                        band.band_width = Convert.ToDouble(bandreader.GetAttribute("bandwidth"));
                                        band.peak_Freq = Convert.ToDouble(bandreader.GetAttribute("peak_Freq"));
                                        band.quant_Level = Convert.ToUInt16(bandreader.GetAttribute("quant_Level"));
                                        this.dsSentry_data.fft_bands.bandList.Add(band);
                                    }
                                }
                                reader.Read();
                                if (reader.NodeType == System.Xml.XmlNodeType.Text)
                                {
                                    values = reader.Value;
                                }
                            }
                            break;
                         */
                        #endregion
                        default: break;
                    }
                }
            }
        }

        public void readXMLDep()
        {
            XmlTextReader reader = new XmlTextReader(fileName);
            string values = "";
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    switch (reader.Name)
                    {
                        case "dsSentry_data":
                            if (reader.HasAttributes)
                            {
                                this.dsSentry_data.manufacturer_Name = reader.GetAttribute("manufacturer_Name");
                                this.dsSentry_data.gateway_Serial = reader.GetAttribute("gateway_Serial");
                                this.dsSentry_data.node_Serial = reader.GetAttribute("node_Serial");
                                this.dsSentry_data.owner_Name = reader.GetAttribute("owner_Name");
                                this.dsSentry_data.location_Name_1 = reader.GetAttribute("location_Name_1");
                                this.dsSentry_data.location_Name_2 = reader.GetAttribute("location_Name_2");
                                this.dsSentry_data.location_Name_3 = reader.GetAttribute("location_Name_3");
                                this.dsSentry_data.gps_Longitude = reader.GetAttribute("gps_Longitude");
                                this.dsSentry_data.gps_Latitude = reader.GetAttribute("gps_Latitude");
                                this.dsSentry_data.sampling_Freq = Convert.ToDouble(reader.GetAttribute("sampling_Freq"));
                                this.dsSentry_data.max_Res_Freq = Convert.ToDouble(reader.GetAttribute("max_Res_Freq"));
                                this.dsSentry_data.offset_Calibration = Convert.ToDouble(reader.GetAttribute("offset_Calibration"));
                                this.dsSentry_data.accel_xCalibration = Convert.ToDouble(reader.GetAttribute("accel_xCalibration"));
                                this.dsSentry_data.accel_yCalibration = Convert.ToDouble(reader.GetAttribute("accel_yCalibration"));
                                this.dsSentry_data.capture_type = reader.GetAttribute("capture_Type");
                                this.dsSentry_data.sched_capture_freq = reader.GetAttribute("scheduled_Capture_Freq");

                                // was boolean
                                this.dsSentry_data.sclk_inversion = reader.GetAttribute("sclk_Inversion");
                                this.dsSentry_data.spi_clk_format = reader.GetAttribute("spi_Clk_Format");
                                this.dsSentry_data.data_first_bit_sel = reader.GetAttribute("data_First_Bit_Selector");
                                this.dsSentry_data.coeff_mem_ct_reset = reader.GetAttribute("coeff_Memory_Count_Reset");
                                this.dsSentry_data.nonoverlap_clk_sel = reader.GetAttribute("nonoverlap_Clk_Selector");
                                this.dsSentry_data.ota_output_buf_enable = reader.GetAttribute("ota_Output_Buffer_Enable");
                                this.dsSentry_data.sf_output_buf_enable = reader.GetAttribute("sf_Output_Buffer_Enable");
                                this.dsSentry_data.input_buf_enable = reader.GetAttribute("input_Buffer_Enable");
                                this.dsSentry_data.real_coeff_mag = reader.GetAttribute("real_Coeff_Magnitude");
                                this.dsSentry_data.real_coeff_sign = reader.GetAttribute("real_Coeff_Sign");
                                this.dsSentry_data.quad_coeff_mag = reader.GetAttribute("quad_Coeff_Magnitude");
                                this.dsSentry_data.quad_coeff_sign = reader.GetAttribute("quad_Coeff_Sign");
                                this.dsSentry_data.sf_output_buf_sel = reader.GetAttribute("sf_Output_Buffer_Selector");
                                this.dsSentry_data.ota_output_buf_sel = reader.GetAttribute("ota_Output_Buffer_Selector");
                                this.dsSentry_data.sentry_enable_bit = reader.GetAttribute("sentry_Enable_Bit");
                                this.dsSentry_data.pll_bias_sel = reader.GetAttribute("pll_Bias_Selector");
                                this.dsSentry_data.sentry_sensitivity = reader.GetAttribute("sentry_Sensitivity");
                                this.dsSentry_data.sentry_clk_src = reader.GetAttribute("sentry_Clk_Source");
                                this.dsSentry_data.clk_out_sel = reader.GetAttribute("clk_Out_Selector");
                                this.dsSentry_data.eff_sample_rate = reader.GetAttribute("effective_Sample_Rate");
                                this.dsSentry_data.decimator_clk_enable = reader.GetAttribute("decimator_Clk_Enable");
                                this.dsSentry_data.decimator_reset = reader.GetAttribute("decimator_Reset");
                                this.dsSentry_data.sig_del_input_mode = reader.GetAttribute("sigma_Delta_Input_Mode");
                                this.dsSentry_data.dc_ss_mod = reader.GetAttribute("spread_Spectrum_Modulator_Dc_Voltage");
                                this.dsSentry_data.adc_ss_mod_sel = reader.GetAttribute("adc_Spread_Spectrum_Modulator_Selector");
                                this.dsSentry_data.dig_out_test_sig_sel = reader.GetAttribute("dig_Out_Test_Signal_Selector");

                                this.dsSentry_data.pll_div_m_reg = Convert.ToDouble(reader.GetAttribute("pll_Divider_M_Register"));
                                this.dsSentry_data.pll_div_n_reg = Convert.ToDouble(reader.GetAttribute("pll_Divider_N_Register"));
                                this.dsSentry_data.num_quant_level = Convert.ToDouble(reader.GetAttribute("num_Quant_Level_Utilized"));
                                this.dsSentry_data.sentry_status = Convert.ToDouble(reader.GetAttribute("sentry_Status"));
                                this.dsSentry_data.atten_pin_ctrl = Convert.ToDouble(reader.GetAttribute("attenuation_Pin_Ctrl"));
                                this.dsSentry_data.nfet_gate_ctrl = Convert.ToDouble(reader.GetAttribute("nfet_Gate_Ctrl"));
                                this.dsSentry_data.org_sentry_dig_test_output = Convert.ToDouble(reader.GetAttribute("sentry_Digital_Test_Output_Origin"));
                                this.dsSentry_data.sig_sel_ss_demod = Convert.ToDouble(reader.GetAttribute("spread_Spectrum_Demodulator_Signal_Selector"));
                                this.dsSentry_data.opt_Field_1 = reader.GetAttribute("opt_Field_1");
                                this.dsSentry_data.opt_Field_2 = reader.GetAttribute("opt_Field_2");
                                this.dsSentry_data.opt_Field_3 = reader.GetAttribute("opt_Field_3");
                                this.dsSentry_data.opt_Field_4 = reader.GetAttribute("opt_Field_4");
                                this.dsSentry_data.opt_Field_5 = reader.GetAttribute("opt_Field_5");
                                this.dsSentry_data.opt_Field_6 = reader.GetAttribute("opt_Field_6");
                                this.dsSentry_data.opt_Field_7 = reader.GetAttribute("opt_Field_7");
                                this.dsSentry_data.opt_Field_8 = reader.GetAttribute("opt_Field_8");

                                // Need method for creating dateTime object from timestamp
                                DateTime dts, dte, dtz;
                                DateTime.TryParse(reader.GetAttribute("timestamp_Start"), out dts);
                                DateTime.TryParse(reader.GetAttribute("timestamp_End"), out dte);
                                DateTime.TryParse(reader.GetAttribute("timezone"), out dtz);
                                this.dsSentry_data.timestamp_End = dte;
                                this.dsSentry_data.timestamp_Start = dts;
                                //this.dsSentry_data.timezone = dtz;

                                // FFT bands
                                // Need method for creating dateTime object from timestamp
                                uint nBands = Convert.ToUInt16(reader.GetAttribute("num_Bands"));
                                this.dsSentry_data.numBands = nBands;
                                //using (XmlReader bandreader = reader.ReadSubtree())
                                //{
                                //reader.Read(); // Need this to get to bands 
                                for (int i = 0; i < nBands; i++)
                                {
                                    //bandreader.ReadStartElement("band");
                                    reader.ReadToFollowing("band_data");
                                    //bandreader.Read();
                                    //bandreader.Read();
                                    _xmlFile._dsSentry_data._band band = new _dsSentry_data._band();
                                    DateTime tmp;
                                    DateTime.TryParse(reader.GetAttribute("timestamp_Created"), out tmp);
                                    band.TimeStampCreated = tmp;
                                    DateTime.TryParse(reader.GetAttribute("timestamp_LastEdit"), out tmp);
                                    band.TimeStampLastEdit = tmp;
                                    band.description = reader.GetAttribute("description");
                                    band.center_Freq = Convert.ToDouble(reader.GetAttribute("center_Freq"));
                                    band.bandwidth = Convert.ToDouble(reader.GetAttribute("bandwidth"));
                                    band.threshold_Level = Convert.ToDouble(reader.GetAttribute("threshold_Level"));
                                    band.quant_Level = Convert.ToUInt16(reader.GetAttribute("quant_Level"));
                                    band.max_Reg_Val = Convert.ToDouble(reader.GetAttribute("max_Reg_Val"));
                                    band.alarm_State = Convert.ToBoolean(reader.GetAttribute("alarm_State"));
                                    this.dsSentry_data.band.times_integrate_pwr = Convert.ToDouble(reader.GetAttribute("times_To_Integrate_Power"));
                                    this.dsSentry_data.band.times_thres_exceed_alarm_trig = Convert.ToDouble(reader.GetAttribute("times_Threshold_Exceed_Alarm_Trigger"));
                                    this.dsSentry_data.band.times_repeat_ref_sig = Convert.ToDouble(reader.GetAttribute("times_To_Repeat_Reference_Signal"));
                                    this.dsSentry_data.bandList.Add(band);
                                }
                                //}

                                // acceleration data
                                accXScale = this.dsSentry_data.accel_xCalibration;
                                accYScale = this.dsSentry_data.accel_yCalibration;
                                accYOffset = this.dsSentry_data.offset_Calibration;
                                if (accXScale == 0) { accXScale = 1; }
                                if (accYScale == 0) { accYScale = 1; }

                                reader.Read();

                                if (reader.NodeType == System.Xml.XmlNodeType.Text)
                                {
                                    values = reader.Value;
                                    this.dsSentry_data.values = parseACCValues(accList, values);
                                    this.dsSentry_data.ACCPPL = accList;
                                }

                                reader.ReadToFollowing("reference_data");
                                this.dsSentry_data.lut_data = Convert.ToDouble(reader.GetAttribute("lut_Data"));
                                this.dsSentry_data.ref_func_mem_map_1 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_1"));
                                this.dsSentry_data.ref_func_mem_map_2 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_2"));
                                this.dsSentry_data.ref_func_mem_map_3 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_3"));
                                this.dsSentry_data.ref_func_mem_map_4 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_4"));
                                this.dsSentry_data.ref_func_mem_map_5 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_5"));
                                this.dsSentry_data.ref_func_mem_map_6 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_6"));
                                this.dsSentry_data.ref_func_mem_map_7 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_7"));
                                this.dsSentry_data.ref_func_mem_map_8 = Convert.ToDouble(reader.GetAttribute("reference_Function_Memory_Map_8"));
                                this.dsSentry_data.sig_del_mod_data_out_reg = Convert.ToDouble(reader.GetAttribute("sigma_Delta_Modulator_Data_Out_Register"));

                            }
                            break;
                        default: break;
                    }
                }
            }
        }

        public void XmlWrite()
        {
            try
            {
                XmlTextWriter writer = new XmlTextWriter(_fileName, Encoding.UTF8);
                writer.WriteStartDocument();
                #region dsSentry_data Element
                writer.WriteStartElement("dsSentry");

                writer.WriteStartElement("sentry_info");
                writer.WriteAttributeString("node_Serial", dsSentry_data.node_Serial);
                writer.WriteAttributeString("gateway_Serial", dsSentry_data.gateway_Serial);
                writer.WriteAttributeString("manufacturer_Name", dsSentry_data.manufacturer_Name);
                writer.WriteAttributeString("owner_Name", dsSentry_data.owner_Name);
                writer.WriteAttributeString("location_Name_1", dsSentry_data.location_Name_1);
                writer.WriteAttributeString("location_Name_2", dsSentry_data.location_Name_2);
                writer.WriteAttributeString("location_Name_3", dsSentry_data.location_Name_3);
                writer.WriteAttributeString("gps_Longitude", dsSentry_data.gps_Longitude);
                writer.WriteAttributeString("gps_Latitude", dsSentry_data.gps_Latitude);
                writer.WriteAttributeString("settings_Timestamp_Created", dsSentry_data.timestamp_Start.ToString("MM/dd/yyyy hh:mm:ss tt K"));
                writer.WriteAttributeString("settings_Timestamp_LastEdit", dsSentry_data.timestamp_End.ToString("MM/dd/yyyy hh:mm:ss tt K"));
                writer.WriteAttributeString("sampling_Freq", dsSentry_data.sampling_Freq.ToString());
                writer.WriteAttributeString("max_Res_Freq", dsSentry_data.max_Res_Freq.ToString());
                writer.WriteAttributeString("offset_Calibration", dsSentry_data.offset_Calibration.ToString());
                writer.WriteAttributeString("accel_xCalibration", dsSentry_data.accel_xCalibration.ToString());
                writer.WriteAttributeString("accel_yCalibration", dsSentry_data.accel_yCalibration.ToString());
                writer.WriteAttributeString("scheduled_Capture_Freq", dsSentry_data.sched_capture_freq);

                // was boolean
                writer.WriteAttributeString("sclk_Inversion", dsSentry_data.sclk_inversion);
                writer.WriteAttributeString("spi_Clk_Format", dsSentry_data.spi_clk_format);
                writer.WriteAttributeString("data_First_Bit_Selector", dsSentry_data.data_first_bit_sel);
                writer.WriteAttributeString("coeff_Memory_Count_Reset", dsSentry_data.coeff_mem_ct_reset);
                writer.WriteAttributeString("nonoverlap_Clk_Selector", dsSentry_data.nonoverlap_clk_sel);
                writer.WriteAttributeString("ota_Output_Buffer_Enable", dsSentry_data.ota_output_buf_enable);
                writer.WriteAttributeString("sf_Output_Buffer_Enable", dsSentry_data.sf_output_buf_enable);
                writer.WriteAttributeString("input_Buffer_Enable", dsSentry_data.input_buf_enable);
                writer.WriteAttributeString("real_Coeff_Magnitude", dsSentry_data.real_coeff_mag);
                writer.WriteAttributeString("real_Coeff_Sign", dsSentry_data.real_coeff_sign);
                writer.WriteAttributeString("quad_Coeff_Magnitude", dsSentry_data.quad_coeff_mag);
                writer.WriteAttributeString("quad_Coeff_Sign", dsSentry_data.quad_coeff_sign);
                writer.WriteAttributeString("sf_Output_Buffer_Selector", dsSentry_data.sf_output_buf_sel);
                writer.WriteAttributeString("ota_Output_Buffer_Selector", dsSentry_data.ota_output_buf_sel);
                writer.WriteAttributeString("sentry_Enable_Bit", dsSentry_data.sentry_enable_bit);
                writer.WriteAttributeString("pll_Bias_Selector", dsSentry_data.pll_bias_sel);
                writer.WriteAttributeString("sentry_Sensitivity", dsSentry_data.sentry_sensitivity);
                writer.WriteAttributeString("sentry_Clk_Source", dsSentry_data.sentry_clk_src);
                writer.WriteAttributeString("clk_Out_Selector", dsSentry_data.clk_out_sel);
                writer.WriteAttributeString("effective_Sample_Rate", dsSentry_data.eff_sample_rate);
                writer.WriteAttributeString("decimator_Clk_Enable", dsSentry_data.decimator_clk_enable);
                writer.WriteAttributeString("decimator_Reset", dsSentry_data.decimator_reset);
                writer.WriteAttributeString("sigma_Delta_Input_Mode", dsSentry_data.sig_del_input_mode);
                writer.WriteAttributeString("spread_Spectrum_Modulator_Dc_Voltage", dsSentry_data.dc_ss_mod);
                writer.WriteAttributeString("adc_Spread_Spectrum_Modulator_Selector", dsSentry_data.adc_ss_mod_sel);
                writer.WriteAttributeString("dig_Out_Test_Signal_Selector", dsSentry_data.dig_out_test_sig_sel);

                writer.WriteAttributeString("pll_Divider_M_Register", dsSentry_data.pll_div_m_reg.ToString());
                writer.WriteAttributeString("pll_Divider_N_Register", dsSentry_data.pll_div_n_reg.ToString());
                writer.WriteAttributeString("num_Quant_Level_Utilized", dsSentry_data.num_quant_level.ToString());
                writer.WriteAttributeString("sentry_Status", dsSentry_data.sentry_status.ToString());
                writer.WriteAttributeString("attenuation_Pin_Ctrl", dsSentry_data.atten_pin_ctrl.ToString());
                writer.WriteAttributeString("nfet_Gate_Ctrl", dsSentry_data.nfet_gate_ctrl.ToString());
                writer.WriteAttributeString("sentry_Digital_Test_Output_Origin", dsSentry_data.org_sentry_dig_test_output.ToString());
                writer.WriteAttributeString("spread_Spectrum_Demodulator_Signal_Selector", dsSentry_data.sig_sel_ss_demod.ToString());
                writer.WriteAttributeString("opt_Field_1", dsSentry_data.opt_Field_1);
                writer.WriteAttributeString("opt_Field_2", dsSentry_data.opt_Field_2);
                writer.WriteAttributeString("opt_Field_3", dsSentry_data.opt_Field_3);
                writer.WriteAttributeString("opt_Field_4", dsSentry_data.opt_Field_4);
                writer.WriteAttributeString("opt_Field_5", dsSentry_data.opt_Field_5);
                writer.WriteAttributeString("opt_Field_6", dsSentry_data.opt_Field_6);
                writer.WriteAttributeString("opt_Field_7", dsSentry_data.opt_Field_7);
                writer.WriteAttributeString("opt_Field_8", dsSentry_data.opt_Field_8);
                writer.WriteEndElement();

                writer.WriteStartElement("sentry_data");
                writer.WriteAttributeString("node_Serial", dsSentry_data.node_Serial);
                writer.WriteAttributeString("timestamp_Start", dsSentry_data.timestamp_Start.ToString("MM/dd/yyyy hh:mm:ss tt K"));
                writer.WriteAttributeString("timestamp_End", dsSentry_data.timestamp_End.ToString("MM/dd/yyyy hh:mm:ss tt K"));
                writer.WriteAttributeString("capture_Type", dsSentry_data.capture_type);
               //  writer.WriteAttributeString("num_Bands", BandObjList.Count.ToString());
                writer.WriteAttributeString("num_Captures_Battery", dsSentry_data.num_captures_battery.ToString());
                writer.WriteAttributeString("num_Alarms_Battery", dsSentry_data.num_alarms_battery.ToString());
                writer.WriteAttributeString("battery_Change_Timestamp", dsSentry_data.battery_change_timestamp.ToString("MM/dd/yyyy hh:mm:ss tt K"));
                writer.WriteAttributeString("sensor_Temp", dsSentry_data.sensor_temp);

                //writer.WriteAttributeString("file_Name", fileName);

                IEnumerator<decimal> acEnum = dsSentry_data.values.GetEnumerator();
                while (acEnum.MoveNext())
                {
                    double val = (double)(acEnum.Current / (decimal)accYScale);
                    writer.WriteValue(val.ToString() + "\r\n"); // To write with current scale
                    // If you want to write the "scaled" value, i.e. the actual value in g's, must set acc_YScale to 1
                    // And do the following:
                    //writer.WriteValue(acEnum.Current + "\r\n");
                }

                writer.WriteEndElement();


                //IEnumerator<BandObj> bandEnum = BandObjList.GetEnumerator();
                //int num = 1;
                //while (bandEnum.MoveNext())
                //{
                //    BandObj band = bandEnum.Current;
                //    writer.WriteStartElement("band_data");
                //    writer.WriteAttributeString("node_Serial", dsSentry_data.node_Serial);
                //    writer.WriteAttributeString("band_Number", num.ToString());
                //    writer.WriteAttributeString("band_Timestamp_Created", band.TimeStamp_Created.ToString());
                //    writer.WriteAttributeString("band_Timestamp_LastEdit", band.TimeStamp_LastEdit.ToString());
                //    writer.WriteAttributeString("description", band.Description);
                //    writer.WriteAttributeString("center_Freq", band.FREQ.ToString());
                //    writer.WriteAttributeString("bandwidth", band.BANDWIDTH.ToString());
                //    writer.WriteAttributeString("threshold_Level", band.ALARM.ToString());
                //    writer.WriteAttributeString("quant_Level", band.QuantLevel.ToString());
                //    writer.WriteAttributeString("max_Reg_Val", band.PEAK_FREQ.ToString());
                //    if (band.BAND_SUM > band.ALARM)
                //        writer.WriteAttributeString("alarm_State", "True");
                //    else
                //        writer.WriteAttributeString("alarm_State", "False");
                //    writer.WriteAttributeString("times_To_Integrate_Power", dsSentry_data.band.times_integrate_pwr.ToString());
                //    writer.WriteAttributeString("times_Threshold_Exceed_Alarm_Trigger", dsSentry_data.band.times_thres_exceed_alarm_trig.ToString());
                //    writer.WriteAttributeString("times_To_Repeat_Reference_Signal", dsSentry_data.band.times_repeat_ref_sig.ToString());
                //    writer.WriteEndElement();
                //    num++;
                //}

                writer.WriteStartElement("reference_data");
                writer.WriteAttributeString("lut_Data", dsSentry_data.lut_data.ToString());
                writer.WriteAttributeString("reference_Function_Memory_Map_1", dsSentry_data.ref_func_mem_map_1.ToString());
                writer.WriteAttributeString("reference_Function_Memory_Map_2", dsSentry_data.ref_func_mem_map_2.ToString());
                writer.WriteAttributeString("reference_Function_Memory_Map_3", dsSentry_data.ref_func_mem_map_3.ToString());
                writer.WriteAttributeString("reference_Function_Memory_Map_4", dsSentry_data.ref_func_mem_map_4.ToString());
                writer.WriteAttributeString("reference_Function_Memory_Map_5", dsSentry_data.ref_func_mem_map_5.ToString());
                writer.WriteAttributeString("reference_Function_Memory_Map_6", dsSentry_data.ref_func_mem_map_6.ToString());
                writer.WriteAttributeString("reference_Function_Memory_Map_7", dsSentry_data.ref_func_mem_map_7.ToString());
                writer.WriteAttributeString("reference_Function_Memory_Map_8", dsSentry_data.ref_func_mem_map_8.ToString());
                writer.WriteAttributeString("sigma_Delta_Modulator_Data_Out_Register", dsSentry_data.sig_del_mod_data_out_reg.ToString());
                writer.WriteEndElement();


                #region
                /*
                #region accel_Values Element
                writer.WriteStartElement("accel_Values");
                writer.WriteAttributeString("timestamp_Start", dsSentry_data.accel_Values.timestamp_Start.ToString());
                writer.WriteAttributeString("timestamp_End", dsSentry_data.accel_Values.timestamp_End.ToString());
                writer.WriteAttributeString("freq", dsSentry_data.accel_Values.freq.ToString());
                writer.WriteAttributeString("freq_Scale", dsSentry_data.accel_Values.freq_Scale);
                writer.WriteAttributeString("type_id", dsSentry_data.accel_Values.type_id);
                writer.WriteAttributeString("name", dsSentry_data.accel_Values.name);
                writer.WriteAttributeString("description", dsSentry_data.accel_Values.description);
                writer.WriteAttributeString("sample_rate", dsSentry_data.accel_Values.sample_rate);
                writer.WriteAttributeString("eng_units", dsSentry_data.accel_Values.eng_units);
                writer.WriteAttributeString("warning_min", dsSentry_data.accel_Values.warning_min.ToString());
                writer.WriteAttributeString("warning_max", dsSentry_data.accel_Values.warning_max.ToString());
                writer.WriteAttributeString("alarm_min", dsSentry_data.accel_Values.alarm_min.ToString());
                writer.WriteAttributeString("alarm_max", dsSentry_data.accel_Values.alarm_max.ToString());
                writer.WriteAttributeString("scaling_type", dsSentry_data.accel_Values.scaling_type);
                writer.WriteAttributeString("tach_id", dsSentry_data.accel_Values.tach_id);
                writer.WriteAttributeString("tach_speed", dsSentry_data.accel_Values.tach_speed.ToString());
                writer.WriteAttributeString("acc_XScale", dsSentry_data.accel_Values.acc_XScale.ToString());
                writer.WriteAttributeString("acc_YScale", dsSentry_data.accel_Values.acc_YScale.ToString());
                writer.WriteAttributeString("acc_YOffset", dsSentry_data.accel_Values.acc_YOffset.ToString());
                writer.WriteAttributeString("max_Res_Freq", dsSentry_data.accel_Values.maxResFreq.ToString());
                IEnumerator<decimal> acEnum = dsSentry_data.accel_Values.values.GetEnumerator();
                while (acEnum.MoveNext())
                {
                    double val = (double)(acEnum.Current / (decimal)accYScale);
                    writer.WriteValue(val.ToString() + "\r\n"); // To write with current scale
                    // If you want to write the "scaled" value, i.e. the actual value in g's, must set acc_YScale to 1
                    // And do the following:
                    //writer.WriteValue(acEnum.Current + "\r\n");
                }
                writer.WriteEndElement();
                #endregion
                #region fft_Values Element
                writer.WriteStartElement("fft_Values");
                writer.WriteAttributeString("timestamp_Start", dsSentry_data.fft_Values.timestamp_Start.ToString());
                writer.WriteAttributeString("timestamp_End", dsSentry_data.fft_Values.timestamp_End.ToString());
                writer.WriteAttributeString("freq", dsSentry_data.fft_Values.freq.ToString());
                writer.WriteAttributeString("freq_Scale", dsSentry_data.fft_Values.freq_Scale);
                writer.WriteAttributeString("type_id", dsSentry_data.fft_Values.type_id);
                writer.WriteAttributeString("name", dsSentry_data.fft_Values.name);
                writer.WriteAttributeString("description", dsSentry_data.fft_Values.description);
                writer.WriteAttributeString("eng_units", dsSentry_data.fft_Values.eng_units);
                writer.WriteAttributeString("warning_min", dsSentry_data.fft_Values.warning_min.ToString());
                writer.WriteAttributeString("warning_max", dsSentry_data.fft_Values.warning_max.ToString());
                writer.WriteAttributeString("alarm_min", dsSentry_data.fft_Values.alarm_min.ToString());
                writer.WriteAttributeString("alarm_max", dsSentry_data.fft_Values.alarm_max.ToString());
                writer.WriteAttributeString("scaling_type", dsSentry_data.fft_Values.scaling_type);
                writer.WriteAttributeString("tach_id", dsSentry_data.fft_Values.tach_id);
                writer.WriteAttributeString("tach_speed", dsSentry_data.fft_Values.tach_speed.ToString());
                writer.WriteAttributeString("fmax", dsSentry_data.fft_Values.fmax.ToString());
                writer.WriteAttributeString("timer_max", dsSentry_data.fft_Values.timer_max.ToString());
                writer.WriteAttributeString("fmin", dsSentry_data.fft_Values.fmin.ToString());
                writer.WriteAttributeString("fft_XScale", dsSentry_data.fft_Values.fft_XScale.ToString());
                writer.WriteAttributeString("fft_YScale", dsSentry_data.fft_Values.fft_YScale.ToString());
                if (dsSentry_data.fft_Values.values != null)
                {
                    IEnumerator<double> ftEnum = dsSentry_data.fft_Values.values.GetEnumerator();
                    while (ftEnum.MoveNext())
                    {
                        writer.WriteValue(ftEnum.Current + "\r\n");
                    }
                }
                writer.WriteEndElement();
                #endregion
                #region fft_bands Element
                writer.WriteStartElement("fft_bands");
                writer.WriteAttributeString("num_Bands", BandObjList.Count.ToString());
                IEnumerator<BandObj> bandEnum = BandObjList.GetEnumerator();
                int num = 1;
                while (bandEnum.MoveNext())
                {
                    BandObj band = bandEnum.Current;
                    writer.WriteStartElement("band");
                    writer.WriteAttributeString("band_Number", num.ToString());
                    writer.WriteAttributeString("timestamp_Created", band.TimeStamp_Created.ToString());
                    writer.WriteAttributeString("timestamp_LastEdit", band.TimeStamp_LastEdit.ToString());
                    writer.WriteAttributeString("description", band.Description);
                    writer.WriteAttributeString("center_Freq", band.FREQ.ToString());
                    writer.WriteAttributeString("threshold", band.ALARM.ToString());
                    writer.WriteAttributeString("bandwidth", band.BANDWIDTH.ToString());
                    writer.WriteAttributeString("quant_Level", band.QuantLevel.ToString());
                    writer.WriteAttributeString("peak_Freq", band.PEAK_FREQ.ToString());
                    writer.WriteEndElement();
                    num++;
                }
                writer.WriteEndElement();
                #endregion*/
                #endregion
                writer.WriteEndElement();
                #endregion


                writer.WriteEndDocument();
                writer.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        public _dsSentry_data dsSentry_data { get { return Sentry_data; } set { Sentry_data = value; } }
        public string fileName { get { return _fileName; } set { _fileName = value; } }
        public string encoding { get { return _encoding; } set { _encoding = value; } }
        public double xmlVersion { get { return _xmlVersion; } set { _xmlVersion = value; } }
        public class _dsSentry_data
        {
            #region
            private _fft_Values fftValues = new _fft_Values();
            private _band bands = new _band();


            private string _manufacturer_name, _gateway_serial, _node_serial, _owner_name;
            private string _location_name_1, _location_name_2, _location_name_3;
            private string _gps_longitude, _gps_latitude;
            private DateTime _timezone, _timestamp_start, _timestamp_end;
            private double _sampling_freq, _max_res_freq;
            private double _offset_calibration, _accel_xcalibration, _accel_ycalibration;

            private string _capture_type, _sched_capture_freq;
            private double _num_captures_battery, _num_alarms_battery;
            private DateTime _battery_change_timestamp;
            private string _sensor_temp;

            // was boolean
            private string _sclk_inversion, _spi_clk_format, _data_first_bit_sel, _coeff_mem_ct_reset, _nonoverlap_clk_sel, _ota_output_buf_enable, _sf_output_buf_enable, _input_buf_enable;

            private double _lut_data, _ref_func_mem_map_1, _ref_func_mem_map_2, _ref_func_mem_map_3, _ref_func_mem_map_4, _ref_func_mem_map_5, _ref_func_mem_map_6, _ref_func_mem_map_7, _ref_func_mem_map_8;

            // was boolean
            private string _real_coeff_mag, _real_coeff_sign, _quad_coeff_mag, _quad_coeff_sign, _sf_output_buf_sel, _ota_output_buf_sel, _sentry_enable_bit, _pll_bias_sel, _sentry_sensitivity;
            private string _sentry_clk_src, _clk_out_sel, _eff_sample_rate, _decimator_clk_enable, _decimator_reset, _sig_del_input_mode, _dc_ss_mod, _adc_ss_mod_sel, _dig_out_test_sig_sel;

            private double _pll_div_m_reg, _pll_div_n_reg, _num_quant_level, _sentry_status, _atten_pin_ctrl, _nfet_gate_ctrl, _org_sentry_dig_test_output, _sig_sel_ss_demod, _sig_del_mod_data_out_reg;

            //private double _bin1_center_freq, _bin2_center_freq, _bin3_center_freq, _bin4_center_freq, _bin5_center_freq, _bin6_center_freq, _bin7_center_freq, _bin8_center_freq;
            //private double _bin1_width, _bin2_width, _bin3_width, _bin4_width, _bin5_width, _bin6_width, _bin7_width, _bin8_width;
            //private double _bin1_thres_lvl, _bin2_thres_lvl, _bin3_thres_lvl, _bin4_thres_lvl, _bin5_thres_lvl, _bin6_thres_lvl, _bin7_thres_lvl, _bin8_thres_lvl;
            //private double _bin1_max_reg, _bin2_max_reg, _bin3_max_reg, _bin4_max_reg, _bin5_max_reg, _bin6_max_reg, _bin7_max_reg, _bin8_max_reg;
            //private bool _bin1_alarm_state, _bin2_alarm_state, _bin3_alarm_state, _bin4_alarm_state, _bin5_alarm_state, _bin6_alarm_state, _bin7_alarm_state, _bin8_alarm_state;
            private string _opt_field_1, _opt_field_2, _opt_field_3, _opt_field_4, _opt_field_5, _opt_field_6, _opt_field_7, _opt_field_8;
            private List<decimal> _values;
            private PointPairList _ACCPPL;
            #endregion
            public _fft_Values fft_Values { get { return fftValues; } set { fftValues = value; } }
            public _band band { get { return bands; } set { bands = value; } }
            public string manufacturer_Name { get { return _manufacturer_name; } set { _manufacturer_name = value; } }
            public string gateway_Serial { get { return _gateway_serial; } set { _gateway_serial = value; } }
            public string node_Serial { get { return _node_serial; } set { _node_serial = value; } }
            public string owner_Name { get { return _owner_name; } set { _owner_name = value; } }
            public string location_Name_1 { get { return _location_name_1; } set { _location_name_1 = value; } }
            public string location_Name_2 { get { return _location_name_2; } set { _location_name_2 = value; } }
            public string location_Name_3 { get { return _location_name_3; } set { _location_name_3 = value; } }
            public string gps_Longitude { get { return _gps_longitude; } set { _gps_longitude = value; } }
            public string gps_Latitude { get { return _gps_latitude; } set { _gps_latitude = value; } }
            //public DateTime timezone { get { return _timezone; } set { _timezone = value; } }
            public DateTime timestamp_Start { get { return _timestamp_start; } set { _timestamp_start = value; } }
            public DateTime timestamp_End { get { return _timestamp_end; } set { _timestamp_end = value; } }
            public double sampling_Freq { get { return _sampling_freq; } set { _sampling_freq = value; } }
            public double max_Res_Freq { get { return _max_res_freq; } set { _max_res_freq = value; } }
            public double offset_Calibration { get { return _offset_calibration; } set { _offset_calibration = value; } }
            public double accel_xCalibration { get { return _accel_xcalibration; } set { _accel_xcalibration = value; } }
            public double accel_yCalibration { get { return _accel_ycalibration; } set { _accel_ycalibration = value; } }

            public double num_captures_battery { get { return _num_captures_battery; } set { _num_captures_battery = value; } }
            public double num_alarms_battery { get { return _num_alarms_battery; } set { _num_alarms_battery = value; } }
            public DateTime battery_change_timestamp { get { return _battery_change_timestamp; } set { _battery_change_timestamp = value; } }
            public string sensor_temp { get { return _sensor_temp; } set { _sensor_temp = value; } }
            public string capture_type { get { return _capture_type; } set { _capture_type = value; } }
            public string sched_capture_freq { get { return _sched_capture_freq; } set { _sched_capture_freq = value; } }
            public string sclk_inversion { get { return _sclk_inversion; } set { _sclk_inversion = value; } }
            public string spi_clk_format { get { return _spi_clk_format; } set { _spi_clk_format = value; } }
            public string data_first_bit_sel { get { return _data_first_bit_sel; } set { _data_first_bit_sel = value; } }
            public string coeff_mem_ct_reset { get { return _coeff_mem_ct_reset; } set { _coeff_mem_ct_reset = value; } }
            public string nonoverlap_clk_sel { get { return _nonoverlap_clk_sel; } set { _nonoverlap_clk_sel = value; } }
            public string ota_output_buf_enable { get { return _ota_output_buf_enable; } set { _ota_output_buf_enable = value; } }
            public string sf_output_buf_enable { get { return _sf_output_buf_enable; } set { _sf_output_buf_enable = value; } }
            public string input_buf_enable { get { return _input_buf_enable; } set { _input_buf_enable = value; } }
            public double lut_data { get { return _lut_data; } set { _lut_data = value; } }
            public double ref_func_mem_map_1 { get { return _ref_func_mem_map_1; } set { _ref_func_mem_map_1 = value; } }
            public double ref_func_mem_map_2 { get { return _ref_func_mem_map_2; } set { _ref_func_mem_map_2 = value; } }
            public double ref_func_mem_map_3 { get { return _ref_func_mem_map_3; } set { _ref_func_mem_map_3 = value; } }
            public double ref_func_mem_map_4 { get { return _ref_func_mem_map_4; } set { _ref_func_mem_map_4 = value; } }
            public double ref_func_mem_map_5 { get { return _ref_func_mem_map_5; } set { _ref_func_mem_map_5 = value; } }
            public double ref_func_mem_map_6 { get { return _ref_func_mem_map_6; } set { _ref_func_mem_map_6 = value; } }
            public double ref_func_mem_map_7 { get { return _ref_func_mem_map_7; } set { _ref_func_mem_map_7 = value; } }
            public double ref_func_mem_map_8 { get { return _ref_func_mem_map_8; } set { _ref_func_mem_map_8 = value; } }
            public string real_coeff_mag { get { return _real_coeff_mag; } set { _real_coeff_mag = value; } }
            public string real_coeff_sign { get { return _real_coeff_sign; } set { _real_coeff_sign = value; } }
            public string quad_coeff_mag { get { return _quad_coeff_mag; } set { _quad_coeff_mag = value; } }
            public string quad_coeff_sign { get { return _quad_coeff_sign; } set { _quad_coeff_sign = value; } }
            public string sf_output_buf_sel { get { return _sf_output_buf_sel; } set { _sf_output_buf_sel = value; } }
            public string ota_output_buf_sel { get { return _ota_output_buf_sel; } set { _ota_output_buf_sel = value; } }
            public string sentry_enable_bit { get { return _sentry_enable_bit; } set { _sentry_enable_bit = value; } }
            public string pll_bias_sel { get { return _pll_bias_sel; } set { _pll_bias_sel = value; } }
            public string sentry_sensitivity { get { return _sentry_sensitivity; } set { _sentry_sensitivity = value; } }
            public string sentry_clk_src { get { return _sentry_clk_src; } set { _sentry_clk_src = value; } }
            public string clk_out_sel { get { return _clk_out_sel; } set { _clk_out_sel = value; } }
            public string eff_sample_rate { get { return _eff_sample_rate; } set { _eff_sample_rate = value; } }
            public string decimator_clk_enable { get { return _decimator_clk_enable; } set { _decimator_clk_enable = value; } }
            public string decimator_reset { get { return _decimator_reset; } set { _decimator_reset = value; } }
            public string sig_del_input_mode { get { return _sig_del_input_mode; } set { _sig_del_input_mode = value; } }
            public string dc_ss_mod { get { return _dc_ss_mod; } set { _dc_ss_mod = value; } }
            public string adc_ss_mod_sel { get { return _adc_ss_mod_sel; } set { _adc_ss_mod_sel = value; } }
            public string dig_out_test_sig_sel { get { return _dig_out_test_sig_sel; } set { _dig_out_test_sig_sel = value; } }
            public double pll_div_m_reg { get { return _pll_div_m_reg; } set { _pll_div_m_reg = value; } }
            public double pll_div_n_reg { get { return _pll_div_n_reg; } set { _pll_div_n_reg = value; } }
            public double num_quant_level { get { return _num_quant_level; } set { _num_quant_level = value; } }
            public double sentry_status { get { return _sentry_status; } set { _sentry_status = value; } }
            public double atten_pin_ctrl { get { return _atten_pin_ctrl; } set { _atten_pin_ctrl = value; } }
            public double nfet_gate_ctrl { get { return _nfet_gate_ctrl; } set { _nfet_gate_ctrl = value; } }
            public double org_sentry_dig_test_output { get { return _org_sentry_dig_test_output; } set { _org_sentry_dig_test_output = value; } }
            public double sig_sel_ss_demod { get { return _sig_sel_ss_demod; } set { _sig_sel_ss_demod = value; } }
            public double sig_del_mod_data_out_reg { get { return _sig_del_mod_data_out_reg; } set { _sig_del_mod_data_out_reg = value; } }

            //public double bin1_Center_Freq { get { return _bin1_center_freq; } set { _bin1_center_freq = value; } }
            //public double bin2_Center_Freq { get { return _bin2_center_freq; } set { _bin2_center_freq = value; } }
            //public double bin3_Center_Freq { get { return _bin3_center_freq; } set { _bin3_center_freq = value; } }
            //public double bin4_Center_Freq { get { return _bin4_center_freq; } set { _bin4_center_freq = value; } }
            //public double bin5_Center_Freq { get { return _bin5_center_freq; } set { _bin5_center_freq = value; } }
            //public double bin6_Center_Freq { get { return _bin6_center_freq; } set { _bin6_center_freq = value; } }
            //public double bin7_Center_Freq { get { return _bin7_center_freq; } set { _bin7_center_freq = value; } }
            //public double bin8_Center_Freq { get { return _bin8_center_freq; } set { _bin8_center_freq = value; } }
            //public double bin1_Width { get { return _bin1_width; } set { _bin1_width = value; } }
            //public double bin2_Width { get { return _bin2_width; } set { _bin2_width = value; } }
            //public double bin3_Width { get { return _bin3_width; } set { _bin3_width = value; } }
            //public double bin4_Width { get { return _bin4_width; } set { _bin4_width = value; } }
            //public double bin5_Width { get { return _bin5_width; } set { _bin5_width = value; } }
            //public double bin6_Width { get { return _bin6_width; } set { _bin6_width = value; } }
            //public double bin7_Width { get { return _bin7_width; } set { _bin7_width = value; } }
            //public double bin8_Width { get { return _bin8_width; } set { _bin8_width = value; } }
            //public double bin1_Thres_Lvl { get { return _bin1_thres_lvl; } set { _bin1_thres_lvl = value; } }
            //public double bin2_Thres_Lvl { get { return _bin2_thres_lvl; } set { _bin2_thres_lvl = value; } }
            //public double bin3_Thres_Lvl { get { return _bin3_thres_lvl; } set { _bin3_thres_lvl = value; } }
            //public double bin4_Thres_Lvl { get { return _bin4_thres_lvl; } set { _bin4_thres_lvl = value; } }
            //public double bin5_Thres_Lvl { get { return _bin5_thres_lvl; } set { _bin5_thres_lvl = value; } }
            //public double bin6_Thres_Lvl { get { return _bin6_thres_lvl; } set { _bin6_thres_lvl = value; } }
            //public double bin7_Thres_Lvl { get { return _bin7_thres_lvl; } set { _bin7_thres_lvl = value; } }
            //public double bin8_Thres_Lvl { get { return _bin8_thres_lvl; } set { _bin8_thres_lvl = value; } }
            //public double bin1_Max_Reg { get { return _bin1_max_reg; } set { _bin1_max_reg = value; } }
            //public double bin2_Max_Reg { get { return _bin2_max_reg; } set { _bin2_max_reg = value; } }
            //public double bin3_Max_Reg { get { return _bin3_max_reg; } set { _bin3_max_reg = value; } }
            //public double bin4_Max_Reg { get { return _bin4_max_reg; } set { _bin4_max_reg = value; } }
            //public double bin5_Max_Reg { get { return _bin5_max_reg; } set { _bin5_max_reg = value; } }
            //public double bin6_Max_Reg { get { return _bin6_max_reg; } set { _bin6_max_reg = value; } }
            //public double bin7_Max_Reg { get { return _bin7_max_reg; } set { _bin7_max_reg = value; } }
            //public double bin8_Max_Reg { get { return _bin8_max_reg; } set { _bin8_max_reg = value; } }
            //public bool bin1_Alarm_State { get { return _bin1_alarm_state; } set { _bin1_alarm_state = value; } }
            //public bool bin2_Alarm_State { get { return _bin2_alarm_state; } set { _bin2_alarm_state = value; } }
            //public bool bin3_Alarm_State { get { return _bin3_alarm_state; } set { _bin3_alarm_state = value; } }
            //public bool bin4_Alarm_State { get { return _bin4_alarm_state; } set { _bin4_alarm_state = value; } }
            //public bool bin5_Alarm_State { get { return _bin5_alarm_state; } set { _bin5_alarm_state = value; } }
            //public bool bin6_Alarm_State { get { return _bin6_alarm_state; } set { _bin6_alarm_state = value; } }
            //public bool bin7_Alarm_State { get { return _bin7_alarm_state; } set { _bin7_alarm_state = value; } }
            //public bool bin8_Alarm_State { get { return _bin8_alarm_state; } set { _bin8_alarm_state = value; } }
            public string opt_Field_1 { get { return _opt_field_1; } set { _opt_field_1 = value; } }
            public string opt_Field_2 { get { return _opt_field_2; } set { _opt_field_2 = value; } }
            public string opt_Field_3 { get { return _opt_field_3; } set { _opt_field_3 = value; } }
            public string opt_Field_4 { get { return _opt_field_4; } set { _opt_field_4 = value; } }
            public string opt_Field_5 { get { return _opt_field_5; } set { _opt_field_5 = value; } }
            public string opt_Field_6 { get { return _opt_field_6; } set { _opt_field_6 = value; } }
            public string opt_Field_7 { get { return _opt_field_7; } set { _opt_field_7 = value; } }
            public string opt_Field_8 { get { return _opt_field_8; } set { _opt_field_8 = value; } }
            public List<decimal> values { get { return _values; } set { _values = value; } }
            public PointPairList ACCPPL { get { return _ACCPPL; } set { _ACCPPL = value; } }

            private uint _numBands = 0;
            private List<_band> _bandList = new List<_band>();

            public List<_band> bandList { get { return _bandList; } set { _bandList = value; } }
            public uint numBands { get { return _numBands; } set { _numBands = value; } }

            public class _band
            {
                private string _desc;
                private double _center_freq, _threshold_level, _bandwidth, _max_reg_val;
                private uint _quant_level;
                private DateTime _tss, _tsle;
                private bool _alarm_state;
                private double _times_integrate_pwr, _times_thres_exceed_alarm_trig, _times_repeat_ref_sig;


                public DateTime TimeStampCreated { get { return _tss; } set { _tss = value; } }
                public DateTime TimeStampLastEdit { get { return _tsle; } set { _tsle = value; } }
                public string description { get { return _desc; } set { _desc = value; } }
                public double center_Freq { get { return _center_freq; } set { _center_freq = value; } }
                public double bandwidth { get { return _bandwidth; } set { _bandwidth = value; } }
                public double threshold_Level { get { return _threshold_level; } set { _threshold_level = value; } }
                public uint quant_Level { get { return _quant_level; } set { _quant_level = value; } }
                public double max_Reg_Val { get { return _max_reg_val; } set { _max_reg_val = value; } }
                public bool alarm_State { get { return _alarm_state; } set { _alarm_state = value; } }
                public double times_integrate_pwr { get { return _times_integrate_pwr; } set { _times_integrate_pwr = value; } }
                public double times_thres_exceed_alarm_trig { get { return _times_thres_exceed_alarm_trig; } set { _times_thres_exceed_alarm_trig = value; } }
                public double times_repeat_ref_sig { get { return _times_repeat_ref_sig; } set { _times_repeat_ref_sig = value; } }
            }

            public class _fft_Values
            {
                private DateTime _timestamp_Start, _timestamp_End;
                private string _freq_Scale, _type_id, _name, _description, _eng_units, _scaling_type, _tach_id;
                private double _freq, _fmax, _timer_max, _fmin, _warning_min, _warning_max, _alarm_min, _alarm_max, _tach_speed, _fft_XScale, _fft_YScale;
                private List<double> _values;
                private PointPairList _FFTValues;

                public string freq_Scale { get { return _freq_Scale; } set { _freq_Scale = value; } }
                public string type_id { get { return _type_id; } set { _type_id = value; } }
                public string description { get { return _description; } set { _description = value; } }
                public string name { get { return _name; } set { _name = value; } }
                public string eng_units { get { return _eng_units; } set { _eng_units = value; } }
                public string scaling_type { get { return _scaling_type; } set { _scaling_type = value; } }
                public string tach_id { get { return _tach_id; } set { _tach_id = value; } }
                public DateTime timestamp_Start { get { return _timestamp_Start; } set { _timestamp_Start = value; } }
                public DateTime timestamp_End { get { return _timestamp_End; } set { _timestamp_End = value; } }
                public double freq { get { return _freq; } set { _freq = value; } }
                public double warning_min { get { return _warning_min; } set { _warning_min = value; } }
                public double alarm_min { get { return _alarm_min; } set { _alarm_min = value; } }
                public double alarm_max { get { return _alarm_max; } set { _alarm_max = value; } }
                public double warning_max { get { return _warning_max; } set { _warning_max = value; } }
                public double tach_speed { get { return _tach_speed; } set { _tach_speed = value; } }
                public double fmax { get { return _fmax; } set { _fmax = value; } }
                public double timer_max { get { return _timer_max; } set { _timer_max = value; } }
                public double fft_XScale { get { return _fft_XScale; } set { _fft_XScale = value; } }
                public double fft_YScale { get { return _fft_YScale; } set { _fft_YScale = value; } }
                public double fmin { get { return _fmin; } set { _fmin = value; } }
                public List<double> values { get { return _values; } set { _values = value; } }
                public PointPairList FFTPPL { get { return _FFTValues; } set { _FFTValues = value; } }
            }
        }


    }

}
