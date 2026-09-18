// Copyright (c) 2024-2026 Pierre G. Boutquin. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

namespace Boutquin.Domain.Enumerations;

using System.ComponentModel;

// Currency is a foundation enumeration shared across every Boutquin.* library so that money,
// pricing, market-data, and tax components all denominate amounts in one canonical currency type
// rather than each defining its own. Members follow ISO 4217: the identifier is the three-letter
// alphabetic code and the numeric value is the ISO 4217 numeric code.

/// <summary>
/// ISO 4217 currency codes. Each member's identifier is the standard three-letter alphabetic
/// code and its numeric value is the corresponding ISO 4217 numeric code, so a consumer can read
/// the numeric code directly from the enum value without a lookup table.
/// </summary>
/// <remarks>
/// <para><b>Pattern:</b> a domain enumeration used as the denomination of the
/// <see cref="Boutquin.Domain.ValueObjects.Money"/> value object. The numeric backing value is
/// the ISO 4217 numeric code, not an ordinal, so values are stable identifiers rather than
/// positional indices.</para>
/// <para><b>Names:</b> the human-readable currency name is carried in a
/// <see cref="DescriptionAttribute"/> on each member and is retrievable with
/// <see cref="Boutquin.Domain.Extensions.EnumExtensions.GetDescription{T}(T)"/>.</para>
/// <para><b>Scope:</b> the catalogue reflects the ISO 4217 list as captured from the source data;
/// it is a denomination reference, not a live feed, and does not track in-year standard
/// amendments (newly assigned or retired codes). Treat it as a stable code set, not an
/// authoritative real-time mirror of the register.</para>
/// </remarks>
public enum Currency
{
    /// <summary>
    /// Sentinel for an unspecified or unknown currency. Not a valid ISO 4217 code; this is the
    /// default value of the enum (numeric <c>0</c>) and denotes the absence of a denomination.
    /// </summary>
    [Description("Not a valid ISO 4217 currency")] Unspecified = 0,

    /// <summary>United Arab Emirates dirham — ISO 4217 alphabetic code <c>AED</c>, numeric <c>784</c>.</summary>
    [Description("United Arab Emirates dirham")] AED = 784,

    /// <summary>Afghan afghani — ISO 4217 alphabetic code <c>AFN</c>, numeric <c>971</c>.</summary>
    [Description("Afghan afghani")] AFN = 971,

    /// <summary>Albanian lek — ISO 4217 alphabetic code <c>ALL</c>, numeric <c>8</c>.</summary>
    [Description("Albanian lek")] ALL = 8,

    /// <summary>Armenian dram — ISO 4217 alphabetic code <c>AMD</c>, numeric <c>51</c>.</summary>
    [Description("Armenian dram")] AMD = 51,

    /// <summary>Netherlands Antillean guilder — ISO 4217 alphabetic code <c>ANG</c>, numeric <c>532</c>.</summary>
    [Description("Netherlands Antillean guilder")] ANG = 532,

    /// <summary>Angolan kwanza — ISO 4217 alphabetic code <c>AOA</c>, numeric <c>973</c>.</summary>
    [Description("Angolan kwanza")] AOA = 973,

    /// <summary>Argentine peso — ISO 4217 alphabetic code <c>ARS</c>, numeric <c>32</c>.</summary>
    [Description("Argentine peso")] ARS = 32,

    /// <summary>Australian dollar — ISO 4217 alphabetic code <c>AUD</c>, numeric <c>36</c>.</summary>
    [Description("Australian dollar")] AUD = 36,

    /// <summary>Aruban florin — ISO 4217 alphabetic code <c>AWG</c>, numeric <c>533</c>.</summary>
    [Description("Aruban florin")] AWG = 533,

    /// <summary>Azerbaijani manat — ISO 4217 alphabetic code <c>AZN</c>, numeric <c>944</c>.</summary>
    [Description("Azerbaijani manat")] AZN = 944,

    /// <summary>Bosnia and Herzegovina convertible mark — ISO 4217 alphabetic code <c>BAM</c>, numeric <c>977</c>.</summary>
    [Description("Bosnia and Herzegovina convertible mark")] BAM = 977,

    /// <summary>Barbados dollar — ISO 4217 alphabetic code <c>BBD</c>, numeric <c>52</c>.</summary>
    [Description("Barbados dollar")] BBD = 52,

    /// <summary>Bangladeshi taka — ISO 4217 alphabetic code <c>BDT</c>, numeric <c>50</c>.</summary>
    [Description("Bangladeshi taka")] BDT = 50,

    /// <summary>Bulgarian lev — ISO 4217 alphabetic code <c>BGN</c>, numeric <c>975</c>.</summary>
    [Description("Bulgarian lev")] BGN = 975,

    /// <summary>Bahraini dinar — ISO 4217 alphabetic code <c>BHD</c>, numeric <c>48</c>.</summary>
    [Description("Bahraini dinar")] BHD = 48,

    /// <summary>Burundian franc — ISO 4217 alphabetic code <c>BIF</c>, numeric <c>108</c>.</summary>
    [Description("Burundian franc")] BIF = 108,

    /// <summary>Bermudian dollar — ISO 4217 alphabetic code <c>BMD</c>, numeric <c>60</c>.</summary>
    [Description("Bermudian dollar")] BMD = 60,

    /// <summary>Brunei dollar — ISO 4217 alphabetic code <c>BND</c>, numeric <c>96</c>.</summary>
    [Description("Brunei dollar")] BND = 96,

    /// <summary>Boliviano — ISO 4217 alphabetic code <c>BOB</c>, numeric <c>68</c>.</summary>
    [Description("Boliviano")] BOB = 68,

    /// <summary>Bolivian Mvdol (funds code) — ISO 4217 alphabetic code <c>BOV</c>, numeric <c>984</c>.</summary>
    [Description("Bolivian Mvdol (funds code)")] BOV = 984,

    /// <summary>Brazilian real — ISO 4217 alphabetic code <c>BRL</c>, numeric <c>986</c>.</summary>
    [Description("Brazilian real")] BRL = 986,

    /// <summary>Bahamian dollar — ISO 4217 alphabetic code <c>BSD</c>, numeric <c>44</c>.</summary>
    [Description("Bahamian dollar")] BSD = 44,

    /// <summary>Bhutanese ngultrum — ISO 4217 alphabetic code <c>BTN</c>, numeric <c>64</c>.</summary>
    [Description("Bhutanese ngultrum")] BTN = 64,

    /// <summary>Botswana pula — ISO 4217 alphabetic code <c>BWP</c>, numeric <c>72</c>.</summary>
    [Description("Botswana pula")] BWP = 72,

    /// <summary>Belarusian ruble — ISO 4217 alphabetic code <c>BYN</c>, numeric <c>933</c>.</summary>
    [Description("Belarusian ruble")] BYN = 933,

    /// <summary>Belize dollar — ISO 4217 alphabetic code <c>BZD</c>, numeric <c>84</c>.</summary>
    [Description("Belize dollar")] BZD = 84,

    /// <summary>Canadian dollar — ISO 4217 alphabetic code <c>CAD</c>, numeric <c>124</c>.</summary>
    [Description("Canadian dollar")] CAD = 124,

    /// <summary>Congolese franc — ISO 4217 alphabetic code <c>CDF</c>, numeric <c>976</c>.</summary>
    [Description("Congolese franc")] CDF = 976,

    /// <summary>WIR Euro (complementary currency) — ISO 4217 alphabetic code <c>CHE</c>, numeric <c>947</c>.</summary>
    [Description("WIR Euro (complementary currency)")] CHE = 947,

    /// <summary>Swiss franc — ISO 4217 alphabetic code <c>CHF</c>, numeric <c>756</c>.</summary>
    [Description("Swiss franc")] CHF = 756,

    /// <summary>WIR Franc (complementary currency) — ISO 4217 alphabetic code <c>CHW</c>, numeric <c>948</c>.</summary>
    [Description("WIR Franc (complementary currency)")] CHW = 948,

    /// <summary>Unidad de Fomento (funds code) — ISO 4217 alphabetic code <c>CLF</c>, numeric <c>990</c>.</summary>
    [Description("Unidad de Fomento (funds code)")] CLF = 990,

    /// <summary>Chilean peso — ISO 4217 alphabetic code <c>CLP</c>, numeric <c>152</c>.</summary>
    [Description("Chilean peso")] CLP = 152,

    /// <summary>Chinese yuan — ISO 4217 alphabetic code <c>CNY</c>, numeric <c>156</c>.</summary>
    [Description("Chinese yuan")] CNY = 156,

    /// <summary>Colombian peso — ISO 4217 alphabetic code <c>COP</c>, numeric <c>170</c>.</summary>
    [Description("Colombian peso")] COP = 170,

    /// <summary>Unidad de Valor Real (UVR) (funds code) — ISO 4217 alphabetic code <c>COU</c>, numeric <c>970</c>.</summary>
    [Description("Unidad de Valor Real (UVR) (funds code)")] COU = 970,

    /// <summary>Costa Rican colon — ISO 4217 alphabetic code <c>CRC</c>, numeric <c>188</c>.</summary>
    [Description("Costa Rican colon")] CRC = 188,

    /// <summary>Cuban peso — ISO 4217 alphabetic code <c>CUP</c>, numeric <c>192</c>.</summary>
    [Description("Cuban peso")] CUP = 192,

    /// <summary>Cape Verde escudo — ISO 4217 alphabetic code <c>CVE</c>, numeric <c>132</c>.</summary>
    [Description("Cape Verde escudo")] CVE = 132,

    /// <summary>Czech koruna — ISO 4217 alphabetic code <c>CZK</c>, numeric <c>203</c>.</summary>
    [Description("Czech koruna")] CZK = 203,

    /// <summary>Djiboutian franc — ISO 4217 alphabetic code <c>DJF</c>, numeric <c>262</c>.</summary>
    [Description("Djiboutian franc")] DJF = 262,

    /// <summary>Danish krone — ISO 4217 alphabetic code <c>DKK</c>, numeric <c>208</c>.</summary>
    [Description("Danish krone")] DKK = 208,

    /// <summary>Dominican peso — ISO 4217 alphabetic code <c>DOP</c>, numeric <c>214</c>.</summary>
    [Description("Dominican peso")] DOP = 214,

    /// <summary>Algerian dinar — ISO 4217 alphabetic code <c>DZD</c>, numeric <c>12</c>.</summary>
    [Description("Algerian dinar")] DZD = 12,

    /// <summary>Egyptian pound — ISO 4217 alphabetic code <c>EGP</c>, numeric <c>818</c>.</summary>
    [Description("Egyptian pound")] EGP = 818,

    /// <summary>Eritrean nakfa — ISO 4217 alphabetic code <c>ERN</c>, numeric <c>232</c>.</summary>
    [Description("Eritrean nakfa")] ERN = 232,

    /// <summary>Ethiopian birr — ISO 4217 alphabetic code <c>ETB</c>, numeric <c>230</c>.</summary>
    [Description("Ethiopian birr")] ETB = 230,

    /// <summary>Euro — ISO 4217 alphabetic code <c>EUR</c>, numeric <c>978</c>.</summary>
    [Description("Euro")] EUR = 978,

    /// <summary>Fiji dollar — ISO 4217 alphabetic code <c>FJD</c>, numeric <c>242</c>.</summary>
    [Description("Fiji dollar")] FJD = 242,

    /// <summary>Falkland Islands pound — ISO 4217 alphabetic code <c>FKP</c>, numeric <c>238</c>.</summary>
    [Description("Falkland Islands pound")] FKP = 238,

    /// <summary>Pound sterling — ISO 4217 alphabetic code <c>GBP</c>, numeric <c>826</c>.</summary>
    [Description("Pound sterling")] GBP = 826,

    /// <summary>Georgian lari — ISO 4217 alphabetic code <c>GEL</c>, numeric <c>981</c>.</summary>
    [Description("Georgian lari")] GEL = 981,

    /// <summary>Ghanaian cedi — ISO 4217 alphabetic code <c>GHS</c>, numeric <c>936</c>.</summary>
    [Description("Ghanaian cedi")] GHS = 936,

    /// <summary>Gibraltar pound — ISO 4217 alphabetic code <c>GIP</c>, numeric <c>292</c>.</summary>
    [Description("Gibraltar pound")] GIP = 292,

    /// <summary>Gambian dalasi — ISO 4217 alphabetic code <c>GMD</c>, numeric <c>270</c>.</summary>
    [Description("Gambian dalasi")] GMD = 270,

    /// <summary>Guinean franc — ISO 4217 alphabetic code <c>GNF</c>, numeric <c>324</c>.</summary>
    [Description("Guinean franc")] GNF = 324,

    /// <summary>Guatemalan quetzal — ISO 4217 alphabetic code <c>GTQ</c>, numeric <c>320</c>.</summary>
    [Description("Guatemalan quetzal")] GTQ = 320,

    /// <summary>Guyanese dollar — ISO 4217 alphabetic code <c>GYD</c>, numeric <c>328</c>.</summary>
    [Description("Guyanese dollar")] GYD = 328,

    /// <summary>Hong Kong dollar — ISO 4217 alphabetic code <c>HKD</c>, numeric <c>344</c>.</summary>
    [Description("Hong Kong dollar")] HKD = 344,

    /// <summary>Honduran lempira — ISO 4217 alphabetic code <c>HNL</c>, numeric <c>340</c>.</summary>
    [Description("Honduran lempira")] HNL = 340,

    /// <summary>Haitian gourde — ISO 4217 alphabetic code <c>HTG</c>, numeric <c>332</c>.</summary>
    [Description("Haitian gourde")] HTG = 332,

    /// <summary>Hungarian forint — ISO 4217 alphabetic code <c>HUF</c>, numeric <c>348</c>.</summary>
    [Description("Hungarian forint")] HUF = 348,

    /// <summary>Indonesian rupiah — ISO 4217 alphabetic code <c>IDR</c>, numeric <c>360</c>.</summary>
    [Description("Indonesian rupiah")] IDR = 360,

    /// <summary>Israeli new shekel — ISO 4217 alphabetic code <c>ILS</c>, numeric <c>376</c>.</summary>
    [Description("Israeli new shekel")] ILS = 376,

    /// <summary>Indian rupee — ISO 4217 alphabetic code <c>INR</c>, numeric <c>356</c>.</summary>
    [Description("Indian rupee")] INR = 356,

    /// <summary>Iraqi dinar — ISO 4217 alphabetic code <c>IQD</c>, numeric <c>368</c>.</summary>
    [Description("Iraqi dinar")] IQD = 368,

    /// <summary>Iranian rial — ISO 4217 alphabetic code <c>IRR</c>, numeric <c>364</c>.</summary>
    [Description("Iranian rial")] IRR = 364,

    /// <summary>Icelandic króna — ISO 4217 alphabetic code <c>ISK</c>, numeric <c>352</c>.</summary>
    [Description("Icelandic króna")] ISK = 352,

    /// <summary>Jamaican dollar — ISO 4217 alphabetic code <c>JMD</c>, numeric <c>388</c>.</summary>
    [Description("Jamaican dollar")] JMD = 388,

    /// <summary>Jordanian dinar — ISO 4217 alphabetic code <c>JOD</c>, numeric <c>400</c>.</summary>
    [Description("Jordanian dinar")] JOD = 400,

    /// <summary>Japanese yen — ISO 4217 alphabetic code <c>JPY</c>, numeric <c>392</c>.</summary>
    [Description("Japanese yen")] JPY = 392,

    /// <summary>Kenyan shilling — ISO 4217 alphabetic code <c>KES</c>, numeric <c>404</c>.</summary>
    [Description("Kenyan shilling")] KES = 404,

    /// <summary>Kyrgyzstani som — ISO 4217 alphabetic code <c>KGS</c>, numeric <c>417</c>.</summary>
    [Description("Kyrgyzstani som")] KGS = 417,

    /// <summary>Cambodian riel — ISO 4217 alphabetic code <c>KHR</c>, numeric <c>116</c>.</summary>
    [Description("Cambodian riel")] KHR = 116,

    /// <summary>Comoro franc — ISO 4217 alphabetic code <c>KMF</c>, numeric <c>174</c>.</summary>
    [Description("Comoro franc")] KMF = 174,

    /// <summary>North Korean won — ISO 4217 alphabetic code <c>KPW</c>, numeric <c>408</c>.</summary>
    [Description("North Korean won")] KPW = 408,

    /// <summary>South Korean won — ISO 4217 alphabetic code <c>KRW</c>, numeric <c>410</c>.</summary>
    [Description("South Korean won")] KRW = 410,

    /// <summary>Kuwaiti dinar — ISO 4217 alphabetic code <c>KWD</c>, numeric <c>414</c>.</summary>
    [Description("Kuwaiti dinar")] KWD = 414,

    /// <summary>Cayman Islands dollar — ISO 4217 alphabetic code <c>KYD</c>, numeric <c>136</c>.</summary>
    [Description("Cayman Islands dollar")] KYD = 136,

    /// <summary>Kazakhstani tenge — ISO 4217 alphabetic code <c>KZT</c>, numeric <c>398</c>.</summary>
    [Description("Kazakhstani tenge")] KZT = 398,

    /// <summary>Lao kip — ISO 4217 alphabetic code <c>LAK</c>, numeric <c>418</c>.</summary>
    [Description("Lao kip")] LAK = 418,

    /// <summary>Lebanese pound — ISO 4217 alphabetic code <c>LBP</c>, numeric <c>422</c>.</summary>
    [Description("Lebanese pound")] LBP = 422,

    /// <summary>Sri Lankan rupee — ISO 4217 alphabetic code <c>LKR</c>, numeric <c>144</c>.</summary>
    [Description("Sri Lankan rupee")] LKR = 144,

    /// <summary>Liberian dollar — ISO 4217 alphabetic code <c>LRD</c>, numeric <c>430</c>.</summary>
    [Description("Liberian dollar")] LRD = 430,

    /// <summary>Lesotho loti — ISO 4217 alphabetic code <c>LSL</c>, numeric <c>426</c>.</summary>
    [Description("Lesotho loti")] LSL = 426,

    /// <summary>Libyan dinar — ISO 4217 alphabetic code <c>LYD</c>, numeric <c>434</c>.</summary>
    [Description("Libyan dinar")] LYD = 434,

    /// <summary>Moroccan dirham — ISO 4217 alphabetic code <c>MAD</c>, numeric <c>504</c>.</summary>
    [Description("Moroccan dirham")] MAD = 504,

    /// <summary>Moldovan leu — ISO 4217 alphabetic code <c>MDL</c>, numeric <c>498</c>.</summary>
    [Description("Moldovan leu")] MDL = 498,

    /// <summary>Malagasy ariary — ISO 4217 alphabetic code <c>MGA</c>, numeric <c>969</c>.</summary>
    [Description("Malagasy ariary")] MGA = 969,

    /// <summary>Macedonian denar — ISO 4217 alphabetic code <c>MKD</c>, numeric <c>807</c>.</summary>
    [Description("Macedonian denar")] MKD = 807,

    /// <summary>Myanmar kyat — ISO 4217 alphabetic code <c>MMK</c>, numeric <c>104</c>.</summary>
    [Description("Myanmar kyat")] MMK = 104,

    /// <summary>Mongolian tögrög — ISO 4217 alphabetic code <c>MNT</c>, numeric <c>496</c>.</summary>
    [Description("Mongolian tögrög")] MNT = 496,

    /// <summary>Macanese pataca — ISO 4217 alphabetic code <c>MOP</c>, numeric <c>446</c>.</summary>
    [Description("Macanese pataca")] MOP = 446,

    /// <summary>Mauritanian ouguiya — ISO 4217 alphabetic code <c>MRU</c>, numeric <c>929</c>.</summary>
    [Description("Mauritanian ouguiya")] MRU = 929,

    /// <summary>Mauritian rupee — ISO 4217 alphabetic code <c>MUR</c>, numeric <c>480</c>.</summary>
    [Description("Mauritian rupee")] MUR = 480,

    /// <summary>Maldivian rufiyaa — ISO 4217 alphabetic code <c>MVR</c>, numeric <c>462</c>.</summary>
    [Description("Maldivian rufiyaa")] MVR = 462,

    /// <summary>Malawian kwacha — ISO 4217 alphabetic code <c>MWK</c>, numeric <c>454</c>.</summary>
    [Description("Malawian kwacha")] MWK = 454,

    /// <summary>Mexican peso — ISO 4217 alphabetic code <c>MXN</c>, numeric <c>484</c>.</summary>
    [Description("Mexican peso")] MXN = 484,

    /// <summary>Mexican Unidad de Inversion (UDI) (funds code) — ISO 4217 alphabetic code <c>MXV</c>, numeric <c>979</c>.</summary>
    [Description("Mexican Unidad de Inversion (UDI) (funds code)")] MXV = 979,

    /// <summary>Malaysian ringgit — ISO 4217 alphabetic code <c>MYR</c>, numeric <c>458</c>.</summary>
    [Description("Malaysian ringgit")] MYR = 458,

    /// <summary>Mozambican metical — ISO 4217 alphabetic code <c>MZN</c>, numeric <c>943</c>.</summary>
    [Description("Mozambican metical")] MZN = 943,

    /// <summary>Namibian dollar — ISO 4217 alphabetic code <c>NAD</c>, numeric <c>516</c>.</summary>
    [Description("Namibian dollar")] NAD = 516,

    /// <summary>Nigerian naira — ISO 4217 alphabetic code <c>NGN</c>, numeric <c>566</c>.</summary>
    [Description("Nigerian naira")] NGN = 566,

    /// <summary>Nicaraguan córdoba — ISO 4217 alphabetic code <c>NIO</c>, numeric <c>558</c>.</summary>
    [Description("Nicaraguan córdoba")] NIO = 558,

    /// <summary>Norwegian krone — ISO 4217 alphabetic code <c>NOK</c>, numeric <c>578</c>.</summary>
    [Description("Norwegian krone")] NOK = 578,

    /// <summary>Nepalese rupee — ISO 4217 alphabetic code <c>NPR</c>, numeric <c>524</c>.</summary>
    [Description("Nepalese rupee")] NPR = 524,

    /// <summary>New Zealand dollar — ISO 4217 alphabetic code <c>NZD</c>, numeric <c>554</c>.</summary>
    [Description("New Zealand dollar")] NZD = 554,

    /// <summary>Omani rial — ISO 4217 alphabetic code <c>OMR</c>, numeric <c>512</c>.</summary>
    [Description("Omani rial")] OMR = 512,

    /// <summary>Panamanian balboa — ISO 4217 alphabetic code <c>PAB</c>, numeric <c>590</c>.</summary>
    [Description("Panamanian balboa")] PAB = 590,

    /// <summary>Peruvian sol — ISO 4217 alphabetic code <c>PEN</c>, numeric <c>604</c>.</summary>
    [Description("Peruvian sol")] PEN = 604,

    /// <summary>Papua New Guinean kina — ISO 4217 alphabetic code <c>PGK</c>, numeric <c>598</c>.</summary>
    [Description("Papua New Guinean kina")] PGK = 598,

    /// <summary>Philippine peso — ISO 4217 alphabetic code <c>PHP</c>, numeric <c>608</c>.</summary>
    [Description("Philippine peso")] PHP = 608,

    /// <summary>Pakistani rupee — ISO 4217 alphabetic code <c>PKR</c>, numeric <c>586</c>.</summary>
    [Description("Pakistani rupee")] PKR = 586,

    /// <summary>Polish zloty — ISO 4217 alphabetic code <c>PLN</c>, numeric <c>985</c>.</summary>
    [Description("Polish zloty")] PLN = 985,

    /// <summary>Paraguayan guaraní — ISO 4217 alphabetic code <c>PYG</c>, numeric <c>600</c>.</summary>
    [Description("Paraguayan guaraní")] PYG = 600,

    /// <summary>Qatari riyal — ISO 4217 alphabetic code <c>QAR</c>, numeric <c>634</c>.</summary>
    [Description("Qatari riyal")] QAR = 634,

    /// <summary>Romanian leu — ISO 4217 alphabetic code <c>RON</c>, numeric <c>946</c>.</summary>
    [Description("Romanian leu")] RON = 946,

    /// <summary>Serbian dinar — ISO 4217 alphabetic code <c>RSD</c>, numeric <c>941</c>.</summary>
    [Description("Serbian dinar")] RSD = 941,

    /// <summary>Russian ruble — ISO 4217 alphabetic code <c>RUB</c>, numeric <c>643</c>.</summary>
    [Description("Russian ruble")] RUB = 643,

    /// <summary>Rwandan franc — ISO 4217 alphabetic code <c>RWF</c>, numeric <c>646</c>.</summary>
    [Description("Rwandan franc")] RWF = 646,

    /// <summary>Saudi riyal — ISO 4217 alphabetic code <c>SAR</c>, numeric <c>682</c>.</summary>
    [Description("Saudi riyal")] SAR = 682,

    /// <summary>Solomon Islands dollar — ISO 4217 alphabetic code <c>SBD</c>, numeric <c>90</c>.</summary>
    [Description("Solomon Islands dollar")] SBD = 90,

    /// <summary>Seychelles rupee — ISO 4217 alphabetic code <c>SCR</c>, numeric <c>690</c>.</summary>
    [Description("Seychelles rupee")] SCR = 690,

    /// <summary>Sudanese pound — ISO 4217 alphabetic code <c>SDG</c>, numeric <c>938</c>.</summary>
    [Description("Sudanese pound")] SDG = 938,

    /// <summary>Swedish krona/kronor — ISO 4217 alphabetic code <c>SEK</c>, numeric <c>752</c>.</summary>
    [Description("Swedish krona/kronor")] SEK = 752,

    /// <summary>Singapore dollar — ISO 4217 alphabetic code <c>SGD</c>, numeric <c>702</c>.</summary>
    [Description("Singapore dollar")] SGD = 702,

    /// <summary>Saint Helena pound — ISO 4217 alphabetic code <c>SHP</c>, numeric <c>654</c>.</summary>
    [Description("Saint Helena pound")] SHP = 654,

    /// <summary>Sierra Leonean leone — ISO 4217 alphabetic code <c>SLE</c>, numeric <c>925</c>.</summary>
    [Description("Sierra Leonean leone")] SLE = 925,

    /// <summary>Somali shilling — ISO 4217 alphabetic code <c>SOS</c>, numeric <c>706</c>.</summary>
    [Description("Somali shilling")] SOS = 706,

    /// <summary>Surinamese dollar — ISO 4217 alphabetic code <c>SRD</c>, numeric <c>968</c>.</summary>
    [Description("Surinamese dollar")] SRD = 968,

    /// <summary>South Sudanese pound — ISO 4217 alphabetic code <c>SSP</c>, numeric <c>728</c>.</summary>
    [Description("South Sudanese pound")] SSP = 728,

    /// <summary>São Tomé and Príncipe dobra — ISO 4217 alphabetic code <c>STN</c>, numeric <c>930</c>.</summary>
    [Description("São Tomé and Príncipe dobra")] STN = 930,

    /// <summary>Salvadoran colón — ISO 4217 alphabetic code <c>SVC</c>, numeric <c>222</c>.</summary>
    [Description("Salvadoran colón")] SVC = 222,

    /// <summary>Syrian pound — ISO 4217 alphabetic code <c>SYP</c>, numeric <c>760</c>.</summary>
    [Description("Syrian pound")] SYP = 760,

    /// <summary>Swazi lilangeni — ISO 4217 alphabetic code <c>SZL</c>, numeric <c>748</c>.</summary>
    [Description("Swazi lilangeni")] SZL = 748,

    /// <summary>Thai baht — ISO 4217 alphabetic code <c>THB</c>, numeric <c>764</c>.</summary>
    [Description("Thai baht")] THB = 764,

    /// <summary>Tajikistani somoni — ISO 4217 alphabetic code <c>TJS</c>, numeric <c>972</c>.</summary>
    [Description("Tajikistani somoni")] TJS = 972,

    /// <summary>Turkmenistani manat — ISO 4217 alphabetic code <c>TMT</c>, numeric <c>934</c>.</summary>
    [Description("Turkmenistani manat")] TMT = 934,

    /// <summary>Tunisian dinar — ISO 4217 alphabetic code <c>TND</c>, numeric <c>788</c>.</summary>
    [Description("Tunisian dinar")] TND = 788,

    /// <summary>Tongan pa'anga — ISO 4217 alphabetic code <c>TOP</c>, numeric <c>776</c>.</summary>
    [Description("Tongan pa'anga")] TOP = 776,

    /// <summary>Turkish lira — ISO 4217 alphabetic code <c>TRY</c>, numeric <c>949</c>.</summary>
    [Description("Turkish lira")] TRY = 949,

    /// <summary>Trinidad and Tobago dollar — ISO 4217 alphabetic code <c>TTD</c>, numeric <c>780</c>.</summary>
    [Description("Trinidad and Tobago dollar")] TTD = 780,

    /// <summary>New Taiwan dollar — ISO 4217 alphabetic code <c>TWD</c>, numeric <c>901</c>.</summary>
    [Description("New Taiwan dollar")] TWD = 901,

    /// <summary>Tanzanian shilling — ISO 4217 alphabetic code <c>TZS</c>, numeric <c>834</c>.</summary>
    [Description("Tanzanian shilling")] TZS = 834,

    /// <summary>Ukrainian hryvnia — ISO 4217 alphabetic code <c>UAH</c>, numeric <c>980</c>.</summary>
    [Description("Ukrainian hryvnia")] UAH = 980,

    /// <summary>Ugandan shilling — ISO 4217 alphabetic code <c>UGX</c>, numeric <c>800</c>.</summary>
    [Description("Ugandan shilling")] UGX = 800,

    /// <summary>United States dollar — ISO 4217 alphabetic code <c>USD</c>, numeric <c>840</c>.</summary>
    [Description("United States dollar")] USD = 840,

    /// <summary>United States dollar (next day) (funds code) — ISO 4217 alphabetic code <c>USN</c>, numeric <c>997</c>.</summary>
    [Description("United States dollar (next day) (funds code)")] USN = 997,

    /// <summary>Uruguay Peso en Unidades Indexadas (URUIURUI) (funds code) — ISO 4217 alphabetic code <c>UYI</c>, numeric <c>940</c>.</summary>
    [Description("Uruguay Peso en Unidades Indexadas (URUIURUI) (funds code)")] UYI = 940,

    /// <summary>Uruguayan peso — ISO 4217 alphabetic code <c>UYU</c>, numeric <c>858</c>.</summary>
    [Description("Uruguayan peso")] UYU = 858,

    /// <summary>Unidad previsional — ISO 4217 alphabetic code <c>UYW</c>, numeric <c>927</c>.</summary>
    [Description("Unidad previsional")] UYW = 927,

    /// <summary>Uzbekistan som — ISO 4217 alphabetic code <c>UZS</c>, numeric <c>860</c>.</summary>
    [Description("Uzbekistan som")] UZS = 860,

    /// <summary>Venezuelan bolívar soberano — ISO 4217 alphabetic code <c>VES</c>, numeric <c>928</c>.</summary>
    [Description("Venezuelan bolívar soberano")] VES = 928,

    /// <summary>Vietnamese dong — ISO 4217 alphabetic code <c>VND</c>, numeric <c>704</c>.</summary>
    [Description("Vietnamese dong")] VND = 704,

    /// <summary>Vanuatu vatu — ISO 4217 alphabetic code <c>VUV</c>, numeric <c>548</c>.</summary>
    [Description("Vanuatu vatu")] VUV = 548,

    /// <summary>Samoan tala — ISO 4217 alphabetic code <c>WST</c>, numeric <c>882</c>.</summary>
    [Description("Samoan tala")] WST = 882,

    /// <summary>CFA franc BEAC — ISO 4217 alphabetic code <c>XAF</c>, numeric <c>950</c>.</summary>
    [Description("CFA franc BEAC")] XAF = 950,

    /// <summary>Silver (one troy ounce) — ISO 4217 alphabetic code <c>XAG</c>, numeric <c>961</c>.</summary>
    [Description("Silver (one troy ounce)")] XAG = 961,

    /// <summary>Gold (one troy ounce) — ISO 4217 alphabetic code <c>XAU</c>, numeric <c>959</c>.</summary>
    [Description("Gold (one troy ounce)")] XAU = 959,

    /// <summary>European Composite Unit (EURCO) (bond market unit) — ISO 4217 alphabetic code <c>XBA</c>, numeric <c>955</c>.</summary>
    [Description("European Composite Unit (EURCO) (bond market unit)")] XBA = 955,

    /// <summary>European Monetary Unit (E.M.U.-6) (bond market unit) — ISO 4217 alphabetic code <c>XBB</c>, numeric <c>956</c>.</summary>
    [Description("European Monetary Unit (E.M.U.-6) (bond market unit)")] XBB = 956,

    /// <summary>European Unit of Account 9 (E.U.A.-9) (bond market unit) — ISO 4217 alphabetic code <c>XBC</c>, numeric <c>957</c>.</summary>
    [Description("European Unit of Account 9 (E.U.A.-9) (bond market unit)")] XBC = 957,

    /// <summary>European Unit of Account 17 (E.U.A.-17) (bond market unit) — ISO 4217 alphabetic code <c>XBD</c>, numeric <c>958</c>.</summary>
    [Description("European Unit of Account 17 (E.U.A.-17) (bond market unit)")] XBD = 958,

    /// <summary>East Caribbean dollar — ISO 4217 alphabetic code <c>XCD</c>, numeric <c>951</c>.</summary>
    [Description("East Caribbean dollar")] XCD = 951,

    /// <summary>Special drawing rights — ISO 4217 alphabetic code <c>XDR</c>, numeric <c>960</c>.</summary>
    [Description("Special drawing rights")] XDR = 960,

    /// <summary>CFA franc BCEAO — ISO 4217 alphabetic code <c>XOF</c>, numeric <c>952</c>.</summary>
    [Description("CFA franc BCEAO")] XOF = 952,

    /// <summary>Palladium (one troy ounce) — ISO 4217 alphabetic code <c>XPD</c>, numeric <c>964</c>.</summary>
    [Description("Palladium (one troy ounce)")] XPD = 964,

    /// <summary>CFP franc (franc Pacifique) — ISO 4217 alphabetic code <c>XPF</c>, numeric <c>953</c>.</summary>
    [Description("CFP franc (franc Pacifique)")] XPF = 953,

    /// <summary>Platinum (one troy ounce) — ISO 4217 alphabetic code <c>XPT</c>, numeric <c>962</c>.</summary>
    [Description("Platinum (one troy ounce)")] XPT = 962,

    /// <summary>SUCRE — ISO 4217 alphabetic code <c>XSU</c>, numeric <c>994</c>.</summary>
    [Description("SUCRE")] XSU = 994,

    /// <summary>Code reserved for testing purposes — ISO 4217 alphabetic code <c>XTS</c>, numeric <c>963</c>.</summary>
    [Description("Code reserved for testing purposes")] XTS = 963,

    /// <summary>ADB Unit of Account — ISO 4217 alphabetic code <c>XUA</c>, numeric <c>965</c>.</summary>
    [Description("ADB Unit of Account")] XUA = 965,

    /// <summary>No currency — ISO 4217 alphabetic code <c>XXX</c>, numeric <c>999</c>.</summary>
    [Description("No currency")] XXX = 999,

    /// <summary>Yemeni rial — ISO 4217 alphabetic code <c>YER</c>, numeric <c>886</c>.</summary>
    [Description("Yemeni rial")] YER = 886,

    /// <summary>South African rand — ISO 4217 alphabetic code <c>ZAR</c>, numeric <c>710</c>.</summary>
    [Description("South African rand")] ZAR = 710,

    /// <summary>Zambian kwacha — ISO 4217 alphabetic code <c>ZMW</c>, numeric <c>967</c>.</summary>
    [Description("Zambian kwacha")] ZMW = 967,

    /// <summary>Zimbabwe Gold — ISO 4217 alphabetic code <c>ZWG</c>, numeric <c>924</c>.</summary>
    [Description("Zimbabwe Gold")] ZWG = 924,
}
