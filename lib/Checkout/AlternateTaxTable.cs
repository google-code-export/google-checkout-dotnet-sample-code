using System;
using System.Collections;

namespace GCheckout.Checkout {
  /// <summary>
  /// Summary description for AlternateTaxTable.
  /// </summary>
  public class AlternateTaxTable {

    /// <summary>
    /// Empty Tax table used for overloads and other "Empty Means"
    /// </summary>
    /// <remarks>
    ///We need a concept of an empty tax table, for overloads and other things.
    ///This is sort of like DBNull.Value for comparisons of AlternateTaxTables
    /// </remarks>
    public static AlternateTaxTable Empty = new AlternateTaxTable();
    
    private string _name;
    private bool _standalone;
    private ArrayList _taxRules = new ArrayList();

    /// <summary>
    /// The name attribute value contains a string that can be used to identify
    /// a tax table or shipping method. The attribute value must be at least
    /// one non-space character and may not be longer than 255 characters.
    /// </summary>
    public string Name {
      get {
        return _name;
      }
      set {
        if (value == null || value == string.Empty) {
          throw new ArgumentNullException("Name", "The Name of the Tax table must not be empty.");   
        }
        if (_name == null || _name == string.Empty) {
          _name = value.ToLower();
        }
        else {
          //The reason for this is it has an impact on every tax in the system.
          //Once you set the name, it should not be changed
          throw new ApplicationException("The name of this Tax" +
            "table has already been set and can't be changed.");   
        }
      }
    }

    /// <summary>
    /// The standalone attribute indicates how taxes should be calculated 
    /// if there is no matching alternate-tax-rule for the given state,
    ///  zip code or country area. If this attribute's value is TRUE and 
    ///  there is no matching alternate-tax-rule, the tax amount will be 
    ///  zero. If the attribute's value is FALSE and there is no matching 
    ///  alternate-tax-rule, the tax amount will be calculated using 
    ///  the default tax table.
    /// </summary>
    public bool StandAlone {
      get {
        return _standalone;
      }
      set {
        _standalone = value;
      }
    }

    /// <summary>
    /// Return the number of rules in the <see cref="AlternateTaxTable"/>
    /// </summary>
    public int RuleCount {
      get {
        return _taxRules.Count; 
      }
    }

    /// <summary>
    /// Create a new Alternate Tax Table
    /// </summary>
    public AlternateTaxTable() {
      _name = string.Empty;
    }

    /// <summary>
    /// Create a new Alternate Tax Table
    /// </summary>
    /// <param name="name">
    /// The name attribute value contains a string that can be used to identify
    /// a tax table or shipping method. The attribute value must be at least
    /// one non-space character and may not be longer than 255 characters.
    /// </param>
    /// <param name="standalone">
    /// The standalone attribute indicates how taxes should be calculated 
    /// if there is no matching alternate-tax-rule for the given state,
    ///  zip code or country area. If this attribute's value is TRUE and 
    ///  there is no matching alternate-tax-rule, the tax amount will be 
    ///  zero. If the attribute's value is FALSE and there is no matching 
    ///  alternate-tax-rule, the tax amount will be calculated using 
    ///  the default tax table.
    /// </param>
    public AlternateTaxTable(string name, bool standalone) {
      Name = name.ToLower();
      StandAlone = standalone;
    }

    /// <summary>
    /// This method adds a tax rule associated with a particular state.
    /// </summary>
    /// <param name="StateCode">This parameter contains a two-letter U.S. state 
    /// code associated with a tax rule.</param>
    /// <param name="TaxRate">The tax rate associated with a tax rule. Tax 
    /// rates are expressed as decimal values. For example, a value of 0.0825 
    /// specifies a tax rate of 8.25%.</param>
    public void AddStateTaxRule(string StateCode, double TaxRate) {
      AutoGen.AlternateTaxRule rule = new AutoGen.AlternateTaxRule();
      rule.rate = TaxRate;
      rule.taxarea = new AutoGen.AlternateTaxRuleTaxarea();
      AutoGen.USStateArea Area = new AutoGen.USStateArea();
      rule.taxarea.Item = Area;
      Area.state = StateCode;
      _taxRules.Add(rule);
    }

    /// <summary>
    /// Adds the country tax rule.
    /// This method adds a tax rule associated with a particular state.
    /// </summary>
    /// <param name="Area">The area.</param>
    /// <param name="TaxRate">The tax rate associated with a tax rule. Tax 
    /// rates are expressed as decimal values. For example, a value of 0.0825 
    /// specifies a tax rate of 8.25%.</param>
    /// <example>
    /// <code>
    ///   // We assume Req is a CheckoutShoppingCartRequest object.
    ///   // Charge the 50 states 8% tax.
    ///   Req.AddCountryTaxRule(AutoGen.USAreas.FULL_50_STATES, 0.08);
    ///   // Charge the 48 continental states 5% tax.
    ///   Req.AddCountryTaxRule(AutoGen.USAreas.CONTINENTAL_48, 0.05);
    ///   // Charge all states (incl territories) 9% tax.
    ///   Req.AddCountryTaxRule(AutoGen.USAreas.ALL, 0.09);
    /// </code>
    /// </example>
    public void AddCountryTaxRule(AutoGen.USAreas Area, double TaxRate) {
      AutoGen.AlternateTaxRule rule = new AutoGen.AlternateTaxRule();
      rule.rate = TaxRate;
      rule.taxarea = new AutoGen.AlternateTaxRuleTaxarea();
      AutoGen.USCountryArea ThisArea = new AutoGen.USCountryArea();
      rule.taxarea.Item = ThisArea;
      ThisArea.countryarea = Area;
      _taxRules.Add(rule);
    }

    /// <summary>
    /// This method adds a tax rule associated with a zip code pattern.
    /// </summary>
    /// <param name="ZipPattern">The zip pattern.</param>
    /// <param name="TaxRate">The tax rate associated with a tax rule. Tax rates 
    /// are expressed as decimal values. For example, a value of 0.0825 
    /// specifies a tax rate of 8.25%.</param>
    public void AddZipTaxRule(string ZipPattern, double TaxRate) {
      if (!CheckoutShoppingCartRequest.IsValidZipPattern(ZipPattern)) {
        throw new ApplicationException(
          CheckoutShoppingCartRequest.ZIP_CODE_PATTERN_EXCEPTION
          );
      }
      AutoGen.AlternateTaxRule rule = new AutoGen.AlternateTaxRule();
      rule.rate = TaxRate;
      rule.taxarea = new AutoGen.AlternateTaxRuleTaxarea();
      AutoGen.USZipArea Area = new AutoGen.USZipArea();
      rule.taxarea.Item = Area;
      Area.zippattern = ZipPattern;
      _taxRules.Add(rule);
    }
    
    /// <summary>
    /// Create the needed table to append to the collection.
    /// </summary>
    /// <returns></returns>
    internal AutoGen.AlternateTaxTable GetAutoGenTable() {
      AutoGen.AlternateTaxTable retVal 
        = new GCheckout.AutoGen.AlternateTaxTable();
      retVal.name = Name;
      retVal.standalone = StandAlone;
      if (StandAlone) {
        retVal.standaloneSpecified = true;   
      }

      AutoGen.AlternateTaxRule[] rules 
        = new GCheckout.AutoGen.AlternateTaxRule[_taxRules.Count];
      _taxRules.CopyTo(rules, 0);
      retVal.alternatetaxrules = rules;
      
      return retVal;
    }
  }
}
